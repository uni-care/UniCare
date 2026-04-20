using MediatR;
using Microsoft.EntityFrameworkCore;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;
using UniCare.Domain.VOs;

namespace UniCare.Application.Item.Commands.CreateItem
{
    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var price = Money.Create(request.Price, request.Currency);

            var item = UniCare.Domain.Aggregates.ItemAggregates.Item.Create(
                request.Title,
                request.Description,
                price,
                request.OwnerId,           
                request.AvailableFrom,     
                request.AvailableTo,       
                request.Location,          
                request.ImageUrls          
            );

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload with navigation properties
            item = await _context.Items
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);

            return new ItemDto(
                item.Id,
                item.Title,
                item.Description,
                item.Price.Amount,
                item.Price.Currency,
                item.Status.ToString(),
                item.OwnerId,
                item.Owner?.FullName ?? string.Empty,
                item.AvailableFrom,
                item.AvailableTo,
                item.Location,
                item.ImageUrls,
                false,   // IsFavorited
                0,       // FavoriteCount
                item.CreatedAt,
                item.UpdatedAt
            );
        }
    }
}