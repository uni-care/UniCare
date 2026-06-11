using MediatR;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;

namespace UniCare.Application.Item.Queries.GetAllItems
{
    public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, List<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;

        public GetAllItemsQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<List<ItemDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var items = await _itemRepository.GetAllAsync(cancellationToken);

            return items.Select(item => ItemDtoMapper.Map(item)).ToList();
        }
    }
}
