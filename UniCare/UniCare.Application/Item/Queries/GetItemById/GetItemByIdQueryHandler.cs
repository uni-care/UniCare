using MediatR;
using Microsoft.EntityFrameworkCore;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Queries.GetItemById
{
    public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
    {
        private readonly IApplicationDbContext _context;

        public GetItemByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemDto> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .Include(i => i.FavoritedBy)
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            var isFavorited = request.CurrentUserId.HasValue &&
                item.FavoritedBy.Any(f => f.UserId == request.CurrentUserId.Value);

            return ItemDtoMapper.Map(item, isFavorited);
        }
    }
}
