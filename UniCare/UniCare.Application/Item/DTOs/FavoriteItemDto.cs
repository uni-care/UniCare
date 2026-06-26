namespace UniCare.Application.Item.DTOs;

public record FavoriteItemDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string Currency,
    string Status,
    Guid OwnerId,
    string OwnerName,
    Guid CategoryId,
    string CategoryName,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string> ImageUrls,
    int FavoriteCount,
    DateTime DateAddedToFavorites,
    DateTime ItemCreatedAt,
    DateTime ItemUpdatedAt
);
