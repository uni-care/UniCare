using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Item.Queries.GetItemById;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class GetItemByIdQueryHandlerTests
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
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(((Item?)null, false));

            var handler = new GetItemByIdQueryHandler(itemRepoMock.Object);
            var query = new GetItemByIdQuery(Guid.NewGuid(), Guid.NewGuid());

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ItemFoundAndFavorited_ReturnsMappedDtoWithFavoriteTrue()
        {
            var item = CreateSampleItem();
            var currentUserId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, currentUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((item, true));

            var handler = new GetItemByIdQueryHandler(itemRepoMock.Object);
            var query = new GetItemByIdQuery(item.Id, currentUserId);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(item.Id, result.Id);
            Assert.Equal(item.Title, result.Title);
            Assert.True(result.IsFavorited);
        }

        [Fact]
        public async Task Handle_ItemFoundAndNotFavorited_ReturnsMappedDtoWithFavoriteFalse()
        {
            var item = CreateSampleItem();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetByIdAsync(item.Id, It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((item, false));

            var handler = new GetItemByIdQueryHandler(itemRepoMock.Object);
            var query = new GetItemByIdQuery(item.Id, null);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(item.Id, result.Id);
            Assert.False(result.IsFavorited);
        }
    }
}