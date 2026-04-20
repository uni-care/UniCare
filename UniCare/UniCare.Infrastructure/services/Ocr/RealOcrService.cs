using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Infrastructure.Settings;

namespace UniCare.Infrastructure.services.Ocr
{
    public class RealOcrService : IOcrService
    {
        private readonly HttpClient _httpClient;
        private readonly OcrSettings _settings;
        private readonly ILogger<RealOcrService> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RealOcrService(
            HttpClient httpClient,
            IOptions<OcrSettings> options,
            ILogger<RealOcrService> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<OcrExtractedDataDto> ExtractStudentDataAsync(
            Stream fileStream,
            string fileName)
        {
            _logger.LogInformation(
                "[RealOcrService] Sending '{FileName}' to OCR endpoint {ApiUrl}",
                fileName, _settings.ApiUrl);

            using var content = new MultipartFormDataContent();

            var fileBytes = await ReadAllBytesAsync(fileStream);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(ResolveMimeType(fileName));

            content.Add(fileContent, "file", fileName);

            HttpResponseMessage httpResponse;
            string rawJson = string.Empty;

            try
            {
                httpResponse = await _httpClient.PostAsync(_settings.ApiUrl, content);
                rawJson = await httpResponse.Content.ReadAsStringAsync();

                _logger.LogDebug(
                    "[RealOcrService] OCR responded HTTP {StatusCode} for '{FileName}'",
                    (int)httpResponse.StatusCode, fileName);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "[RealOcrService] Network error for '{FileName}'", fileName);
                return EmptyResult(rawJson: null);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,
                    "[RealOcrService] Request timed out for '{FileName}'", fileName);
                return EmptyResult(rawJson: null);
            }

            // Non-2xx  →  save raw body for audit, return Pending so the
            // handler records the submission without auto-rejecting the user.
            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "[RealOcrService] Non-success HTTP {StatusCode} for '{FileName}'. Body: {Body}",
                    (int)httpResponse.StatusCode, fileName, rawJson);
                return EmptyResult(rawJson);
            }

            OcrApiResponse? apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<OcrApiResponse>(rawJson, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "[RealOcrService] JSON parse failed for '{FileName}'. Raw: {Raw}",
                    fileName, rawJson);
                return EmptyResult(rawJson);
            }

            // API-level failure 
            if (apiResponse is null || !apiResponse.Success || apiResponse.Data is null)
            {
                _logger.LogWarning(
                    "[RealOcrService] success=false or empty data for '{FileName}'. Error: {Err}",
                    fileName, apiResponse?.Error ?? "unknown");
                return EmptyResult(rawJson);
            }

            var verdict = apiResponse.Data.is_approved;

            var dto = new OcrExtractedDataDto
            {
                ExtractedUniversity = Normalise(apiResponse.Data.University),
                ExtractedFaculty = Normalise(apiResponse.Data.Faculty),
                is_approved = apiResponse.Data.is_approved,
                RawApiResponse = rawJson
            };

            _logger.LogInformation(
                "[RealOcrService] OCR done for '{FileName}'. " +
                "is_approved={is_approved} Name={Name} Uni={Uni} Faculty={Fac} Expiry={Exp}",
                fileName, dto.is_approved,
                dto.ExtractedUniversity, dto.ExtractedFaculty);

            return dto;
        }

   

        private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
        {
            if (stream is MemoryStream ms)
                return ms.ToArray();

            using var buffer = new MemoryStream();
            await stream.CopyToAsync(buffer);
            return buffer.ToArray();
        }

        private static string ResolveMimeType(string fileName) =>
            Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

        private static string? Normalise(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static DateTime? ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            string[] formats = ["yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "dd-MM-yyyy"];

            if (DateTime.TryParseExact(raw, formats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var parsed))
                return DateTime.SpecifyKind(parsed, DateTimeKind.Utc);

            if (DateTime.TryParse(raw, out var fallback))
                return DateTime.SpecifyKind(fallback.Date, DateTimeKind.Utc);

            return null;
        }
        private static OcrExtractedDataDto EmptyResult(string? rawJson) => new()
        {
            is_approved = null,  // safe default on failure
            RawApiResponse = rawJson
        };
    }
}
