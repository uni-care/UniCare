using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Item.Commands.ToggleFavorite;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class ToggleFavoriteCommandHandlerTests
    {
        private static Item CreateSampleItem()
        {
            return Item.Create(
                "Sample item",
                "Sample description",
                Money.Create(100m, "USD"),
                ItemType.ForSale,
                Guid.NewGuid(),
                Guid.NewGuid()
            );
        }

        [Fact]
        public async Task Handle_ItemNotFound_ThrowsKeyNotFoundException()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Item?)null);

            var handler = new ToggleFavoriteCommandHandler(itemRepoMock.Object);
            var command = new ToggleFavoriteCommand(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_NoExistingFavorite_AddsFavoriteAndReturnsTrue()
        {
            var item = CreateSampleItem();
            var userId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.GetFavoriteAsync(userId, item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserFavorite?)null);

            var handler = new ToggleFavoriteCommandHandler(itemRepoMock.Object);
            var command = new ToggleFavoriteCommand(item.Id, userId);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            itemRepoMock.Verify(r => r.AddFavoriteAsync(userId, item.Id, It.IsAny<CancellationToken>()), Times.Once);
            itemRepoMock.Verify(r => r.RemoveFavoriteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ExistingFavorite_RemovesFavoriteAndReturnsFalse()
        {
            var item = CreateSampleItem();
            var userId = Guid.NewGuid();
            var existingFavorite = UserFavorite.Create(userId, item.Id);

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            itemRepoMock
                .Setup(r => r.GetFavoriteAsync(userId, item.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingFavorite);

            var handler = new ToggleFavoriteCommandHandler(itemRepoMock.Object);
            var command = new ToggleFavoriteCommand(item.Id, userId);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            itemRepoMock.Verify(r => r.RemoveFavoriteAsync(userId, item.Id, It.IsAny<CancellationToken>()), Times.Once);
            itemRepoMock.Verify(r => r.AddFavoriteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}