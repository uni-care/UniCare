namespace UniCare.Application.Item.DTOs;

public static class ItemDtoMapper
{
    public static ItemDto Map(Domain.Aggregates.ItemAggregates.Item item, bool isFavorited = false, int? favoriteCount = null) =>
        new(
            item.Id,
            item.Title,
            item.Description,
            item.Price.Amount,
            item.Price.Currency,
            (int)item.ItemType,
            item.Status.ToString(),
            item.OwnerId,
            item.Owner?.FullName ?? string.Empty,
            item.CategoryId,
            item.Category?.Name ?? string.Empty,
            item.AvailableFrom,
            item.AvailableTo,
            item.Location,
            item.ImageUrls,
            isFavorited,
            favoriteCount ?? item.FavoritedBy?.Count ?? 0,
            item.CreatedAt,
            item.UpdatedAt
        );
}
