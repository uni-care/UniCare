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
            var (items, totalCount) = await _itemRepository.GetPagedAsync(
                 request.PageNumber,
                 request.PageSize,
                 ItemStatus.Draft,
                 cancellationToken);
            var dtos = items.Select(item => ItemDtoMapper.Map(item)).ToList();


            return new PaginatedList<ItemDto>(dtos, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
