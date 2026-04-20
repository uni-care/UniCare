using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniCare.Infrastructure.services.Ocr
{
    public class OcrApiRequest
    {
        public string ContentType { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;
    }
    public class OcrApiResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; init; }

        [JsonPropertyName("data")]
        public OcrApiData? Data { get; init; }

        [JsonPropertyName("error")]
        public string? Error { get; init; }
    }
    public class OcrApiData
    {
      

        [JsonPropertyName("university")]
        public string? University { get; init; }

        [JsonPropertyName("faculty")]
        public string? Faculty { get; init; }

        [JsonPropertyName("is_approved")]
        public bool? is_approved { get; init; }
    }
}
