using UniCare.Domain.Enums;

namespace UniCare.Api.Models
{
    public record CreateItemRequest(
    string Title,
    string Description,
    decimal Price,
    ItemType ItemType,
    string Currency,
    Guid CategoryId,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
