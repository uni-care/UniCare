using MediatR;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Queries.GetFavorites;

public class GetFavoritesQuery : PaginationParams, IRequest<PaginatedList<FavoriteItemDto>>
{
    public Guid UserId { get; set; }
    public Guid? CategoryId { get; set; }
    public string SortBy { get; set; } = "dateAdded";
    public string SortDirection { get; set; } = "desc";
}