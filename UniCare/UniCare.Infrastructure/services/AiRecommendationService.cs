using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Interfaces;

namespace UniCare.Infrastructure.services
{
    public class AiRecommendationService : IAiRecommendationService
    {
        private readonly HttpClient _httpClient;

        public AiRecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Guid>> GetRecommendedIdsAsync(string prompt, CancellationToken ct)
        {
            var requestBody = new { prompt = prompt, limit = 10 };

            var response = await _httpClient.PostAsJsonAsync("v1/recommendations", requestBody, ct);

            if (!response.IsSuccessStatusCode)
                return new List<Guid>();

            var result = await response.Content.ReadFromJsonAsync<AiRecommendationResponse>(cancellationToken: ct);
            return result?.ItemIds ?? new List<Guid>();
        }
    }
}
