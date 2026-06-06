using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Queries.GetAiRecommendations
{
    public class GetAiRecommendationsHandler : IRequestHandler<GetAiRecommendationsQuery, List<ItemDto>>
    {
        private readonly IAiRecommendationService _aiService;
        private readonly IApplicationDbContext _context;

        public GetAiRecommendationsHandler(IAiRecommendationService aiService, IApplicationDbContext context)
        {
            _aiService = aiService;
            _context = context;
        }

        public async Task<List<ItemDto>> Handle(GetAiRecommendationsQuery request, CancellationToken ct)
        {
            // 1. Get IDs from AI
            var ids = await _aiService.GetRecommendedIdsAsync(request.Prompt, ct);

            if (!ids.Any()) return new List<ItemDto>();

            var itemsList = await _context.Items
                .Where(i => ids.Contains(i.Id))
                .ToListAsync(ct);

            var items = itemsList.Select(i => {
                // Handle comma-separated images as per your infrastructure

                return new ItemDto(
                    i.Id,                   
                    i.Title,                
                    i.Description,          
                    i.Price.Amount,         
                    i.Price.Currency,       
                    "StatusPlaceholder",    
                    i.OwnerId,              
                    "OwnerName",            
                    null,                   
                    null,                   
                    null,
                    i.ImageUrls ?? new List<string>(),              
                    false,                  
                    0,                      
                    DateTime.UtcNow,        
                    DateTime.UtcNow         
                );
            }).ToList();

            // 4. Re-sort to maintain AI's relevance order
            return ids.Select(id => items.FirstOrDefault(i => i.Id == id))
                      .Where(i => i != null)
                      .ToList()!;
        }
    }
}
