using MediatR;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;

namespace UniCare.Application.Item.Queries.GetAllItems
{
    public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, PaginatedList<ItemDto>>
    {
        private readonly IItemRepository _itemRepository;

        public GetAllItemsQueryHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<PaginatedList<ItemDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount, favoritedItemIds) = await _itemRepository.GetPagedAsync(
                 request.PageNumber,
                 request.PageSize,
                 request.ItemType,
                 ItemStatus.Draft,
                 request.CurrentUserId,
                 request.CategoryId,
                 request.IsFree,
                 request.AvailableOnly,
                 cancellationToken);
            var dtos = items.Select(item => ItemDtoMapper.Map(item, favoritedItemIds.Contains(item.Id))).ToList();

            return new PaginatedList<ItemDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
