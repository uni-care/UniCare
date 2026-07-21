using MediatR;
using UniCare.Application.Interfaces;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Queries.GetAiRecommendations
{
    public class GetAiRecommendationsHandler : IRequestHandler<GetAiRecommendationsQuery, List<ItemDto>>
    {
        private readonly IAiRecommendationService _aiService;
        private readonly IItemRepository _itemRepository;

        public GetAiRecommendationsHandler(IAiRecommendationService aiService, IItemRepository itemRepository)
        {
            _aiService = aiService;
            _itemRepository = itemRepository;
        }

        public async Task<List<ItemDto>> Handle(GetAiRecommendationsQuery request, CancellationToken ct)
        {
            var ids = await _aiService.GetRecommendedIdsAsync(request.Prompt, ct);

            if (!ids.Any()) return new List<ItemDto>();

            var itemsList = await _itemRepository.GetByIdsAsync(ids, ct);

            var items = itemsList.Select(i => ItemDtoMapper.Map(i)).ToList();

            return ids.Select(id => items.FirstOrDefault(i => i.Id == id))
                      .Where(i => i != null)
                      .ToList()!;
        }
    }
}