using MediatR;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Enums;

namespace UniCare.Application.Item.Commands.CreateItem
{
    public record CreateItemCommand(
        string Title,
        string Description,
        decimal Price,
        ItemType ItemType,
        string Currency,
        Guid CategoryId,
        DateTime? AvailableFrom,
        DateTime? AvailableTo,
        string? Location,
        List<string>? ImageUrls,
        Guid OwnerId
    ) : IRequest<ItemDto>;
}
