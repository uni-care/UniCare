using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniCare.Application.Item.Queries.GetFavorites;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Interfaces;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class GetFavoritesQueryHandlerTests
    {
        [Fact]
        public async Task Handle_DescendingSortDirection_PassesDescendingTrueToRepository()
        {
            var userId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetFavoritesPagedAsync(
                    userId, null, "dateAdded", true, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<UserFavorite>(), 0));

            var handler = new GetFavoritesQueryHandler(itemRepoMock.Object);
            var query = new GetFavoritesQuery
            {
                UserId = userId,
                PageNumber = 1,
                PageSize = 10,
                CategoryId = null,
                SortBy = "dateAdded",
                SortDirection = "desc"
            };

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            itemRepoMock.Verify(r => r.GetFavoritesPagedAsync(
                userId, null, "dateAdded", true, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AscendingSortDirection_PassesDescendingFalseToRepository()
        {
            var userId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetFavoritesPagedAsync(
                    userId, null, "dateAdded", false, 1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<UserFavorite>(), 0));

            var handler = new GetFavoritesQueryHandler(itemRepoMock.Object);
            var query = new GetFavoritesQuery
            {
                UserId = userId,
                PageNumber = 1,
                PageSize = 10,
                CategoryId = null,
                SortBy = "dateAdded",
                SortDirection = "asc"
            };

            await handler.Handle(query, CancellationToken.None);

            itemRepoMock.Verify(r => r.GetFavoritesPagedAsync(
                userId, null, "dateAdded", false, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithCategoryFilter_PassesCategoryIdToRepository()
        {
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.GetFavoritesPagedAsync(
                    userId, categoryId, "dateAdded", true, 2, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((new List<UserFavorite>(), 0));

            var handler = new GetFavoritesQueryHandler(itemRepoMock.Object);
            var query = new GetFavoritesQuery
            {
                UserId = userId,
                PageNumber = 2,
                PageSize = 5,
                CategoryId = categoryId,
                SortBy = "dateAdded",
                SortDirection = "desc"
            };

            await handler.Handle(query, CancellationToken.None);

            itemRepoMock.Verify(r => r.GetFavoritesPagedAsync(
                userId, categoryId, "dateAdded", true, 2, 5, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}