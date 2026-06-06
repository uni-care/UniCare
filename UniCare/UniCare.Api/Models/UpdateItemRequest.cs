namespace UniCare.Api.Models
{
    public record UpdateItemRequest(
    string? Title,
    string? Description,
    decimal? Price,
    string? Currency,
    Guid? CategoryId,
    string? Status,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls
);
}
