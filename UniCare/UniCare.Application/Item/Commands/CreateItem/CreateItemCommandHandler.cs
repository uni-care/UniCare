using MediatR;
using UniCare.Application.Common.Exceptions;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;
using UniCare.Domain.VOs;

namespace UniCare.Application.Item.Commands.CreateItem
{
    public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, ItemDto>
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CreateItemCommandHandler(IItemRepository itemRepository, ICategoryRepository categoryRepository)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ItemDto> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken);
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

            await _itemRepository.AddAsync(item, cancellationToken);

            var createdItem = await _itemRepository.GetItemWithDetailsAsync(item.Id, cancellationToken);

            return ItemDtoMapper.Map(createdItem!);
        }
    }
}