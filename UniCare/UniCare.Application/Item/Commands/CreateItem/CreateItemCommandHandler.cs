using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;

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

            var item = Item.Create(
                request.Title,
                request.Description,
                price,
                request.OwnerId,
                request.CategoryId,
                request.AvailableFrom,
                request.AvailableTo,
                request.Location,
                request.ImageUrls
            );

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            // Reload with navigation properties
            await _context.Entry(item)
                .Reference(i => i.Owner)
                .LoadAsync(cancellationToken);

            await _context.Entry(item)
                .Reference(i => i.Category)
                .LoadAsync(cancellationToken);

            return new ItemDto(
                item.Id,
                item.Title,
                item.Description,
                item.Price.Amount,
                item.Price.Currency,
                item.Status.ToString(),
                item.OwnerId,
                item.Owner.FullName,
                item.CategoryId,
                item.Category.Name,
                item.AvailableFrom,
                item.AvailableTo,
                item.Location,
                item.ImageUrls,
                false,
                0,
                item.CreatedAt,
                item.UpdatedAt
            );
        }
    }
}
