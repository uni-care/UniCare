using MediatR;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Queries.GetFavorites;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PaginatedList<FavoriteItemDto>>
{
    private readonly IItemRepository _itemRepository;

    public GetFavoritesQueryHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<PaginatedList<FavoriteItemDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var descending = request.SortDirection.ToLowerInvariant() == "desc";

        var (favorites, totalCount) = await _itemRepository.GetFavoritesPagedAsync(
            request.UserId,
            request.CategoryId,
            request.SortBy,
            descending,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = favorites.Select(ItemDtoMapper.MapToFavoriteDto).ToList();

        return new PaginatedList<FavoriteItemDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}