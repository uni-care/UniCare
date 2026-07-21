using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Item.Commands.UpdateItem;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class UpdateItemCommandHandlerTests
    {
        private static Item CreateSampleItem(Guid ownerId, Guid categoryId, decimal price = 100m, string currency = "USD")
        {
            return Item.Create(
                "Original title",
                "Original description",
                Money.Create(price, currency),
                ItemType.ForSale,
                ownerId,
                categoryId
            );
        }

        [Fact]
        public async Task Handle_ItemNotFound_ThrowsKeyNotFoundException()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Item?)null);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                Guid.NewGuid(), Guid.NewGuid(), null, null, null, null, null, null, null, null, null, null, null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RequestingUserNotOwner_ThrowsUnauthorizedAccessException()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, Guid.NewGuid());

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                item.Id, Guid.NewGuid(), "New title", null, null, null, null, null, null, null, null, null, null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_CategoryNotFound_ThrowsKeyNotFoundException()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, Guid.NewGuid());

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var categoryRepoMock = new Mock<ICategoryRepository>();
            categoryRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, categoryRepoMock.Object);

            var command = new UpdateItemCommand(
                item.Id, ownerId, null, null, null, null, null, Guid.NewGuid(), null, null, null, null, null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidUpdate_UpdatesFieldsAndReturnsDto()
        {
            var ownerId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, categoryId);

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                item.Id, ownerId, "Updated title", "Updated description", null, null, null, null,
                null, null, null, null, null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("Updated title", result.Title);
            Assert.Equal("Updated description", result.Description);
            itemRepoMock.Verify(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PriceUpdateWithoutCurrency_UsesExistingCurrency()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, Guid.NewGuid(), price: 50m, currency: "EGP");

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                item.Id, ownerId, null, null, 75m, null, null, null, null, null, null, null, null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(75m, item.Price.Amount);
            Assert.Equal("EGP", item.Price.Currency);
        }

        [Fact]
        public async Task Handle_ValidStatusString_UpdatesStatus()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, Guid.NewGuid());

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                item.Id, ownerId, null, null, null, null, null, null, "Available", null, null, null, null);

            await handler.Handle(command, CancellationToken.None);

            Assert.Equal(ItemStatus.Available, item.Status);
        }

        [Fact]
        public async Task Handle_InvalidStatusString_DoesNotThrowAndLeavesStatusUnchanged()
        {
            var ownerId = Guid.NewGuid();
            var item = CreateSampleItem(ownerId, Guid.NewGuid());
            var originalStatus = item.Status;

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.UpdateAsync(item, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var handler = new UpdateItemCommandHandler(itemRepoMock.Object, Mock.Of<ICategoryRepository>());

            var command = new UpdateItemCommand(
                item.Id, ownerId, null, null, null, null, null, null, "NotARealStatus", null, null, null, null);

            await handler.Handle(command, CancellationToken.None);

            Assert.Equal(originalStatus, item.Status);
        }
    }
}