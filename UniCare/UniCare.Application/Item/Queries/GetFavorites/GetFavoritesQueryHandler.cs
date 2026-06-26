using MediatR;
using Microsoft.EntityFrameworkCore;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Queries.GetFavorites;

public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, Result<PagedResult<FavoriteItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetFavoritesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedResult<FavoriteItemDto>>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.UserFavorites
            .AsNoTracking()
            .Where(uf => uf.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var categoryLower = request.Category.Trim().ToLower();
            query = query.Where(uf => uf.Item.Category.Name.ToLower() == categoryLower);
        }

        if (request.MinPrice.HasValue)
            query = query.Where(uf => uf.Item.Price.Amount >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(uf => uf.Item.Price.Amount <= request.MaxPrice.Value);

        var sortBy = request.SortBy.ToLowerInvariant();
        var descending = request.SortDirection.ToLowerInvariant() == "desc";

        query = (sortBy, descending) switch
        {
            ("price", false)     => query.OrderBy(uf => uf.Item.Price.Amount),
            ("price", true)      => query.OrderByDescending(uf => uf.Item.Price.Amount),
            ("name", false)      => query.OrderBy(uf => uf.Item.Title),
            ("name", true)       => query.OrderByDescending(uf => uf.Item.Title),
            ("dateadded", false) => query.OrderBy(uf => uf.CreatedAt),
            _                    => query.OrderByDescending(uf => uf.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(uf => new FavoriteItemDto(
                uf.Item.Id,
                uf.Item.Title,
                uf.Item.Description,
                uf.Item.Price.Amount,
                uf.Item.Price.Currency,
                uf.Item.Status.ToString(),
                uf.Item.OwnerId,
                uf.Item.Owner != null ? uf.Item.Owner.FullName : string.Empty,
                uf.Item.CategoryId,
                uf.Item.Category != null ? uf.Item.Category.Name : string.Empty,
                uf.Item.AvailableFrom,
                uf.Item.AvailableTo,
                uf.Item.Location,
                uf.Item.ImageUrls,
                uf.Item.FavoritedBy.Count(),
                uf.CreatedAt,
                uf.Item.CreatedAt,
                uf.Item.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        var pagedResult = PagedResult<FavoriteItemDto>.Create(items, request.PageNumber, request.PageSize, totalCount);

        return Result<PagedResult<FavoriteItemDto>>.Success(pagedResult);
    }
}
