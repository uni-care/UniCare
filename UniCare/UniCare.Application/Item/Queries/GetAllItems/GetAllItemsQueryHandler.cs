using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Repositories;

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

            return items.Select(item => new ItemDto(
                 item.Id,
    item.Title,
    item.Description,
    item.Price.Amount,
    item.Price.Currency,
    item.Status.ToString(),
    item.OwnerId,
    item.Owner?.FullName ?? string.Empty,
    item.AvailableFrom, // Ensure these are present
    item.AvailableTo,
    item.Location,
    item.ImageUrls,
    false, // IsFavorited
    0,     // FavoriteCount
    item.CreatedAt,
    item.UpdatedAt
            )).ToList();
        }
    }
}
