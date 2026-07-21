using MediatR;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Commands.ToggleFavorite
{
    public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
    {
        private readonly IItemRepository _itemRepository;

        public ToggleFavoriteCommandHandler(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            var existingFavorite = await _itemRepository.GetFavoriteAsync(request.UserId, request.ItemId, cancellationToken);

            if (existingFavorite != null)
            {
                await _itemRepository.RemoveFavoriteAsync(request.UserId, request.ItemId, cancellationToken);
                return false;
            }
            else
            {
                await _itemRepository.AddFavoriteAsync(request.UserId, request.ItemId, cancellationToken);
                return true;
            }
        }
    }
}