using MediatR;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Queries.GetFavorites;

public record GetFavoritesQuery(
    Guid UserId,
    int PageNumber,
    int PageSize,
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    string SortBy,
    string SortDirection
) : IRequest<Result<PagedResult<FavoriteItemDto>>>;
