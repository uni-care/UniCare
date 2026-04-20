namespace UniCare.Api.Models
{
    public record CreateItemRequest(
    string Title,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
