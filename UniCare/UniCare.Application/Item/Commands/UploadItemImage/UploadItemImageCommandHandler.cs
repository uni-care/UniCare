using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Application.Interfaces;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.Item.Commands.UploadItemImage
{

    public sealed class UploadItemImageCommandHandler
        : ICommandHandler<UploadItemImageCommand, Result<UploadItemImageResult>>
    {
        private readonly IApplicationDbContext _db;
        private readonly IFileStorageService _storage;

        public UploadItemImageCommandHandler(
            IApplicationDbContext db,
            IFileStorageService storage)
        {
            _db = db;
            _storage = storage;
        }

        public async Task<Result<UploadItemImageResult>> Handle(
            UploadItemImageCommand command,
            CancellationToken cancellationToken)
        {
            var item = await _db.Items
                .FirstOrDefaultAsync(i => i.Id == command.ItemId, cancellationToken);

            if (item is null)
                return Result<UploadItemImageResult>.NotFound(
                    $"Item {command.ItemId} not found.");

            if (item.OwnerId != command.RequestingUserId)
                return Result<UploadItemImageResult>.Forbidden(
                    "You are not authorised to upload images for this item.");

            var folder = $"unicare/items/{command.ItemId}";
            var uploadResult = await _storage.UploadAsync(
                command.FileContent, command.FileName, folder, cancellationToken);

            // Append the new URL to the item's image list
            var updatedUrls = item.ImageUrls.ToList();
            updatedUrls.Add(uploadResult.Url);
            item.Update(imageUrls: updatedUrls);

            await _db.SaveChangesAsync(cancellationToken);

            return Result<UploadItemImageResult>.Success(
                new UploadItemImageResult(uploadResult.Url, uploadResult.PublicId));
        }
    }
}
