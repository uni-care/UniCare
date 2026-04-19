using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.Item.Commands.ToggleFavorite
{
    public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ToggleFavoriteCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.Items
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            var existingFavorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.ItemId == request.ItemId, cancellationToken);

            if (existingFavorite != null)
            {
                // Remove from favorites
                _context.UserFavorites.Remove(existingFavorite);
                await _context.SaveChangesAsync(cancellationToken);
                return false; // Unfavorited
            }
            else
            {
                // Add to favorites
                var favorite = UserFavorite.Create(request.UserId, request.ItemId);
                _context.UserFavorites.Add(favorite);
                await _context.SaveChangesAsync(cancellationToken);
                return true; // Favorited
            }
        }
    }
}
