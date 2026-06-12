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
            if (request.Price.HasValue && !string.IsNullOrWhiteSpace(request.Currency))
                price = Money.Create(request.Price.Value, request.Currency);

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

            if (!string.IsNullOrEmpty(request.Status) &&
                Enum.TryParse<ItemStatus>(request.Status, ignoreCase: true, out var status))
            {
                item.UpdateStatus(status);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return await _context.Items
                .AsNoTracking()
                .Where(i => i.Id == request.ItemId)
                .Select(i => new ItemDto(
                    i.Id,
                    i.Title,
                    i.Description,
                    i.Price.Amount,
                    i.Price.Currency,
                    i.Status.ToString(),
                    i.OwnerId,
                    i.Owner != null ? i.Owner.FullName : string.Empty,
                    i.CategoryId,
                    i.Category != null ? i.Category.Name : string.Empty,
                    i.AvailableFrom,
                    i.AvailableTo,
                    i.Location,
                    i.ImageUrls,
                    i.FavoritedBy.Any(f => f.UserId == request.RequestingUserId),
                    i.FavoritedBy.Count(),
                    i.CreatedAt,
                    i.UpdatedAt
                ))
                .FirstAsync(cancellationToken);
        }
    }
}
