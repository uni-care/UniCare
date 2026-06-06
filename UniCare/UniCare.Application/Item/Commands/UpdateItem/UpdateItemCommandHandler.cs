using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Enums;
using UniCare.Domain.Interfaces;
using UniCare.Domain.VOs;

namespace UniCare.Application.Item.Commands.UpdateItem
{
    public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .Include(i => i.Owner)
                .Include(i => i.FavoritedBy)
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            if (item.OwnerId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You are not authorized to update this item.");

            Money? price = null;
            if (request.Price.HasValue && !string.IsNullOrEmpty(request.Currency))
            {
                price = Money.Create(request.Price.Value, request.Currency);
            }

            item.Update(
                request.Title,
                request.Description,
                price,
                request.CategoryId,
                request.AvailableFrom,
                request.AvailableTo,
                request.Location,
                request.ImageUrls
            );

            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ItemStatus>(request.Status, out var status))
            {
                item.UpdateStatus(status);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new ItemDto(
                item.Id,
                item.Title,
                item.Description,
                item.Price.Amount,
                item.Price.Currency,
                item.Status.ToString(),
                item.OwnerId,
                item.Owner.FullName,
                item.AvailableFrom,
                item.AvailableTo,
                item.Location,
                item.ImageUrls,
                item.FavoritedBy.Any(f => f.UserId == request.RequestingUserId),
                item.FavoritedBy.Count,
                item.CreatedAt,
                item.UpdatedAt
            );
        }
    }
}
