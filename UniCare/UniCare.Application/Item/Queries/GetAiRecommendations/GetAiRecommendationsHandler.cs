using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var ids = await _aiService.GetRecommendedIdsAsync(request.Prompt, ct);

            if (!ids.Any()) return new List<ItemDto>();

            var itemsList = await _context.Items
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .Where(i => ids.Contains(i.Id))
                .ToListAsync(ct);

            var items = itemsList.Select(i => ItemDtoMapper.Map(i)).ToList();

            return ids.Select(id => items.FirstOrDefault(i => i.Id == id))
                      .Where(i => i != null)
                      .ToList()!;
        }
    }
}
