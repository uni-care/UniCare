using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Item.Queries.GetAllItems;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class GetAllItemsQueryHandlerTests
    {
        private static Item CreateSampleItem(string title = "Sample item")
        {
            return Item.Create(
                title,
                "Sample description",
                Money.Create(100m, "USD"),
                ItemType.ForSale,
                Guid.NewGuid(),
                Guid.NewGuid()
            );
        }

        [Fact]
        public async Task Handle_NoItems_ReturnsEmptyPaginatedList()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetPagedAsync(
                    1, 10, null, ItemStatus.Draft, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Item>(), 0, new HashSet<Guid>()));

            var handler = new GetAllItemsQueryHandler(itemRepoMock.Object);
            var query = new GetAllItemsQuery { PageNumber = 1, PageSize = 10 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Handle_ItemsReturned_MapsFavoritedStatusCorrectly()
        {
            var item1 = CreateSampleItem("Favorited item");
            var item2 = CreateSampleItem("Not favorited item");
            var currentUserId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetPagedAsync(
                    1, 10, null, ItemStatus.Draft, currentUserId, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Item> { item1, item2 }, 2, new HashSet<Guid> { item1.Id }));

            var handler = new GetAllItemsQueryHandler(itemRepoMock.Object);
            var query = new GetAllItemsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CurrentUserId = currentUserId
            };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.Items.Count);
            Assert.True(result.Items.First(i => i.Id == item1.Id).IsFavorited);
            Assert.False(result.Items.First(i => i.Id == item2.Id).IsFavorited);
        }

        [Fact]
        public async Task Handle_PassesItemTypeFilterToRepository()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetPagedAsync(
                    1, 10, ItemType.ForRent, ItemStatus.Draft, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Item>(), 0, new HashSet<Guid>()));

            var handler = new GetAllItemsQueryHandler(itemRepoMock.Object);
            var query = new GetAllItemsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                ItemType = ItemType.ForRent
            };

            await handler.Handle(query, CancellationToken.None);

            itemRepoMock.Verify(r => r.GetPagedAsync(
                1, 10, ItemType.ForRent, ItemStatus.Draft, null, null, null, null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_PassesCategoryAndIsFreeAndAvailableOnlyFiltersToRepository()
        {
            var categoryId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetPagedAsync(
                    1, 10, null, ItemStatus.Draft, null, categoryId, true, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Item>(), 0, new HashSet<Guid>()));

            var handler = new GetAllItemsQueryHandler(itemRepoMock.Object);
            var query = new GetAllItemsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CategoryId = categoryId,
                IsFree = true,
                AvailableOnly = true
            };

            await handler.Handle(query, CancellationToken.None);

            itemRepoMock.Verify(r => r.GetPagedAsync(
                1, 10, null, ItemStatus.Draft, null, categoryId, true, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnsCorrectPageNumberAndPageSize()
        {
            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetPagedAsync(
                    2, 5, null, ItemStatus.Draft, null, null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<Item>(), 12, new HashSet<Guid>()));

            var handler = new GetAllItemsQueryHandler(itemRepoMock.Object);
            var query = new GetAllItemsQuery { PageNumber = 2, PageSize = 5 };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal(2, result.PageNumber);
            Assert.Equal(5, result.PageSize);
            Assert.Equal(12, result.TotalCount);
        }
    }
}