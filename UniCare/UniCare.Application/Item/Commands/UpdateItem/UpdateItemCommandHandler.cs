using MediatR;
using Microsoft.EntityFrameworkCore;
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
                .Include(i => i.Category)
                .Include(i => i.FavoritedBy)
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            if (item.OwnerId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You are not authorized to update this item.");

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == request.CategoryId.Value, cancellationToken);

                if (!categoryExists)
                    throw new KeyNotFoundException($"Category with ID {request.CategoryId.Value} not found.");
            }

            Money? price = null;
            if (request.Price.HasValue && !string.IsNullOrEmpty(request.Currency))
            {
                price = Money.Create(request.Price.Value, request.Currency);
            }

            item.Update(
                request.Title,
                request.Description,
                price,
                request.ItemType,
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

            item = await _context.Items
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .Include(i => i.FavoritedBy)
                .FirstAsync(i => i.Id == request.ItemId, cancellationToken);

            return ItemDtoMapper.Map(
                item,
                item.FavoritedBy.Any(f => f.UserId == request.RequestingUserId)
            );
        }
    }
}
