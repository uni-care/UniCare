using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Commands.CreateItem
{
    public record CreateItemCommand(
    string Title,
    string Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string>? ImageUrls,
    Guid OwnerId
) : IRequest<ItemDto>;
}
