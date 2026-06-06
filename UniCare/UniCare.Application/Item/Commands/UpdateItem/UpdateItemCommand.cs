using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Commands.UpdateItem
{
    public record UpdateItemCommand(
    Guid ItemId,
    Guid RequestingUserId,
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
) : IRequest<ItemDto>;
}
