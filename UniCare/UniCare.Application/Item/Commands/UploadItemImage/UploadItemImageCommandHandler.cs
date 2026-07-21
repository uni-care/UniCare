using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Application.Interfaces;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Commands.UploadItemImage
{
    public sealed class UploadItemImageCommandHandler
        : ICommandHandler<UploadItemImageCommand, Result<UploadItemImageResult>>
    {
        private readonly IItemRepository _itemRepository;
        private readonly IFileStorageService _storage;

        public UploadItemImageCommandHandler(
            IItemRepository itemRepository,
            IFileStorageService storage)
        {
            _itemRepository = itemRepository;
            _storage = storage;
        }

        public async Task<Result<UploadItemImageResult>> Handle(
            UploadItemImageCommand command,
            CancellationToken cancellationToken)
        {
            var item = await _itemRepository.GetByIdAsync(command.ItemId, cancellationToken);

            if (item is null)
                return Result<UploadItemImageResult>.NotFound(
                    $"Item {command.ItemId} not found.");

            if (item.OwnerId != command.RequestingUserId)
                return Result<UploadItemImageResult>.Forbidden(
                    "You are not authorised to upload images for this item.");

            var folder = $"unicare/items/{command.ItemId}";
            var uploadResult = await _storage.UploadAsync(
                command.FileContent, command.FileName, folder, cancellationToken);

            var updatedUrls = item.ImageUrls.ToList();
            updatedUrls.Add(uploadResult.Url);
            item.Update(imageUrls: updatedUrls);

            await _itemRepository.UpdateAsync(item, cancellationToken);

            return Result<UploadItemImageResult>.Success(
                new UploadItemImageResult(uploadResult.Url, uploadResult.PublicId));
        }
    }
}