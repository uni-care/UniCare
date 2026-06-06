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

            return await ExtractStudentDataAsync(
                fileStream, fileName,
                userId: string.Empty,
                docType: "student_id");
        }


        public async Task<OcrExtractedDataDto> ExtractStudentDataAsync(
            Stream fileStream,
            string fileName,
            string userId,
            string docType)
        {
            _logger.LogInformation(
                "[RealOcrService] Sending '{FileName}' (userId={UserId}, docType={DocType}) to {ApiUrl}",
                fileName, userId, docType, _settings.ApiUrl);

            using var form = new MultipartFormDataContent();

            form.Add(new StringContent(userId), "user_id");

            form.Add(new StringContent(docType), "doc_type");

            var fileBytes = await ReadAllBytesAsync(fileStream);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType =
                new MediaTypeHeaderValue(ResolveMimeType(fileName));
            //form.Add(fileContent, "file", fileName);

            HttpResponseMessage httpResponse;
            string rawJson = string.Empty;

            try
            {
                httpResponse = await _httpClient.PostAsync(_settings.ApiUrl, form);
                rawJson = await httpResponse.Content.ReadAsStringAsync();

                _logger.LogDebug(
                    "[RealOcrService] HTTP {StatusCode} for '{FileName}'",
                    (int)httpResponse.StatusCode, fileName);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "[RealOcrService] Network error for '{FileName}'", fileName);
                return PendingResult(rawJson: null);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex,
                    "[RealOcrService] Timeout for '{FileName}'", fileName);
                return PendingResult(rawJson: null);
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "[RealOcrService] HTTP {StatusCode} for '{FileName}'. Body: {Body}",
                    (int)httpResponse.StatusCode, fileName, rawJson);
                return PendingResult(rawJson);
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
                return PendingResult(rawJson);
            }

            if (apiResponse is null)
            {
                _logger.LogWarning(
                    "[RealOcrService] Null response body for '{FileName}'", fileName);
                return PendingResult(rawJson);
            }

            var verdict = apiResponse.IsApproved ? OcrVerdict.Verified : OcrVerdict.Rejected;

            var dto = new OcrExtractedDataDto
            {
                ExtractedUniversity = Normalise(apiResponse.University),
                ExtractedFaculty = Normalise(apiResponse.Faculty),
                Verdict = verdict,
                RawApiResponse = rawJson
            };

            _logger.LogInformation(
                "[RealOcrService] Done for '{FileName}'. " +
                "Verdict={Verdict} University='{Uni}' Faculty='{Fac}'",
                fileName, dto.Verdict, dto.ExtractedUniversity, dto.ExtractedFaculty);

            return dto;
        }

        private static async Task<byte[]> ReadAllBytesAsync(Stream stream)
        {
            if (stream is MemoryStream ms) return ms.ToArray();
            using var buf = new MemoryStream();
            await stream.CopyToAsync(buf);
            return buf.ToArray();
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
        private static OcrExtractedDataDto PendingResult(string? rawJson) => new()
        {
            Verdict = OcrVerdict.Pending,
            RawApiResponse = rawJson
        };
    }
}
