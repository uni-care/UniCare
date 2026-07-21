using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Common;
using UniCare.Application.Interfaces;
using UniCare.Application.Item.Commands.UploadItemImage;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class UploadItemImageCommandHandlerTests
    {
        private static Item CreateSampleItem(Guid ownerId)
        {
            return Item.Create(
                "Sample item",
                "Sample description",
                Money.Create(100m, "USD"),
                ItemType.ForSale,
                ownerId,
                Guid.NewGuid()
            );
        }

        [Fact]
        public async Task Handle_ItemNotFound_ReturnsNotFoundResult()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Item?)null);

            var handler = new UploadItemImageCommandHandler(itemRepoMock.Object, Mock.Of<IFileStorageService>());

            var command = new UploadItemImageCommand(
                Guid.NewGuid(), Guid.NewGuid(), new byte[] { 1, 2, 3 }, "photo.jpg", "image/jpeg");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task Handle_RequestingUserNotOwner_ReturnsForbiddenResult()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId);

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UploadItemImageCommandHandler(itemRepoMock.Object, Mock.Of<IFileStorageService>());

            var command = new UploadItemImageCommand(
                item.Id, Guid.NewGuid(), new byte[] { 1, 2, 3 }, "photo.jpg", "image/jpeg");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public async Task Handle_ValidUpload_UploadsFileAndAppendsUrlToItem()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId);
            var fileContent = new byte[] { 1, 2, 3 };

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var storageMock = new Mock<IFileStorageService>();
            storageMock
                .Setup(s => s.UploadAsync(fileContent, "photo.jpg", $"unicare/items/{item.Id}", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FileUploadResult("https://cdn.example.com/photo.jpg", "public-id-123"));

            var handler = new UploadItemImageCommandHandler(itemRepoMock.Object, storageMock.Object);

            var command = new UploadItemImageCommand(
                item.Id, ownerId, fileContent, "photo.jpg", "image/jpeg");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("https://cdn.example.com/photo.jpg", result.Data!.Url);
            Assert.Equal("public-id-123", result.Data.PublicId);
            Assert.Contains("https://cdn.example.com/photo.jpg", item.ImageUrls);
            itemRepoMock.Verify(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidUpload_CallsStorageServiceWithCorrectFolder()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId);
            var fileContent = new byte[] { 9, 9, 9 };

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var storageMock = new Mock<IFileStorageService>();
            storageMock
                .Setup(s => s.UploadAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FileUploadResult("https://cdn.example.com/x.jpg", "id-x"));

            var handler = new UploadItemImageCommandHandler(itemRepoMock.Object, storageMock.Object);

            var command = new UploadItemImageCommand(item.Id, ownerId, fileContent, "x.jpg", "image/jpeg");

            await handler.Handle(command, CancellationToken.None);

            storageMock.Verify(s => s.UploadAsync(
                fileContent, "x.jpg", $"unicare/items/{item.Id}", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}