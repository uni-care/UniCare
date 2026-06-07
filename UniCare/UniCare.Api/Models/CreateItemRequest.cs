namespace UniCare.Api.Models
{
    public record CreateItemRequest(
    string Title,
    string Description,
    decimal Price,
    string Currency,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
