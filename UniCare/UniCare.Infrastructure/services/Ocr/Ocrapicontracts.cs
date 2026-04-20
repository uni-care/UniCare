using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UniCare.Infrastructure.services.Ocr
{
    public class OcrApiResponse
    {
        [JsonPropertyName("university")]
        public string? University { get; init; }

        [JsonPropertyName("faculty")]
        public string? Faculty { get; init; }

        [JsonPropertyName("is_approved")]
        public bool IsApproved { get; init; }
    }
}
