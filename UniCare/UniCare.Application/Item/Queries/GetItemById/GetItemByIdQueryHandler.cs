using MediatR;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;

namespace UniCare.Application.Item.Queries.GetItemById
{
    public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
    {
        private readonly IItemRepository _itemRepository;

        public GetItemByIdQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<ItemDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var (item, isFavorited) = await _itemRepository.GetByIdAsync(
                request.ItemId,
                request.CurrentUserId,
                cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            return ItemDtoMapper.Map(item, isFavorited);
        }
    }
}