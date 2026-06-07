namespace UniCare.Api.Models
{
    public record UpdateItemRequest(
    string? Title,
    string? Description,
    decimal? Price,
    string? Currency,
    string? Status,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
