using UniCare.Domain.Enums;

namespace UniCare.Api.Models
{
    public record UpdateItemRequest(
    string? Title,
    string? Description,
    decimal? Price,
    ItemType? ItemType,
    string? Currency,
    Guid? CategoryId,
    string? Status,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
