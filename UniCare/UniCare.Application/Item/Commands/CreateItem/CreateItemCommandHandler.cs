using MediatR;
using Microsoft.EntityFrameworkCore;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Interfaces;
using UniCare.Domain.VOs;
using UniCare.Application.Common.Exceptions;


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
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
            Console.WriteLine($"Looking for CategoryId: {request.CategoryId}");
            if (!categoryExists)
                throw new NotFoundException(nameof(Category), request.CategoryId);

            var price = Money.Create(request.Price, request.Currency);

            var item = Domain.Aggregates.ItemAggregates.Item.Create(
                request.Title,
                request.Description,
                price,
                request.ItemType,
                request.OwnerId,
                request.CategoryId,
                request.AvailableFrom,
                request.AvailableTo,
                request.Location,
                request.ImageUrls
            );

            _context.Items.Add(item);
            await _context.SaveChangesAsync(cancellationToken);

            item = await _context.Items
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .FirstAsync(i => i.Id == item.Id, cancellationToken);

            return ItemDtoMapper.Map(item);
        }
    }
}
