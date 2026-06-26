using MediatR;
using Microsoft.EntityFrameworkCore;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Enums;
using UniCare.Domain.Interfaces;
using UniCare.Domain.VOs;
using UniCare.Domain.Repositories;

namespace UniCare.Application.Item.Commands.UpdateItem
{
    public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, ItemDto>
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateItemCommandHandler(IItemRepository itemRepository, ICategoryRepository categoryRepository)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ItemDto> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(request.ItemId, cancellationToken);

            if (item == null)
                throw new KeyNotFoundException($"Item with ID {request.ItemId} not found.");

            if (item.OwnerId != request.RequestingUserId)
                throw new UnauthorizedAccessException("You are not authorized to update this item.");

            if (request.CategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId.Value, cancellationToken);

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

            await _itemRepository.UpdateAsync(item, cancellationToken);

            var updatedItem = await _itemRepository.GetItemWithDetailsAsync(request.ItemId, cancellationToken);

            return new ItemDto(
                updatedItem!.Id,
                updatedItem.Title,
                updatedItem.Description,
                updatedItem.Price.Amount,
                updatedItem.Price.Currency,
                updatedItem.Status.ToString(),
                updatedItem.OwnerId,
                updatedItem.Owner?.FullName ?? string.Empty,
                updatedItem.CategoryId,
                updatedItem.Category?.Name ?? string.Empty,
                updatedItem.AvailableFrom,
                updatedItem.AvailableTo,
                updatedItem.Location,
                updatedItem.ImageUrls,
                updatedItem.FavoritedBy.Any(f => f.UserId == request.RequestingUserId),
                updatedItem.FavoritedBy.Count,
                updatedItem.CreatedAt,
                updatedItem.UpdatedAt
            );
        }
    }
}
