using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Common.Exceptions;
using UniCare.Application.Item.Commands.CreateItem;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Items
{
    public class CreateItemCommandHandlerTests
    {
        private static CreateItemCommand ValidCommand(Guid categoryId, Guid ownerId) => new(
            "Bike for sale",
            "A slightly used mountain bike",
            100m,
            ItemType.ForSale,
            "USD",
            categoryId,
            null,
            null,
            "Cairo",
            null,
            ownerId
        );

        [Fact]
        public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
        {
            var categoryId = Guid.NewGuid();

            var categoryRepoMock = new Mock<ICategoryRepository>();
            categoryRepoMock
                .Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new CreateItemCommandHandler(Mock.Of<IItemRepository>(), categoryRepoMock.Object);

            var command = ValidCommand(categoryId, Guid.NewGuid());

            var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(command, CancellationToken.None));

            Assert.Contains(categoryId.ToString(), ex.Message);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesItemAndReturnsMappedDto()
        {
            var categoryId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            Item? capturedItem = null;

            var categoryRepoMock = new Mock<ICategoryRepository>();
            categoryRepoMock
                .Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .Callback<Item, CancellationToken>((item, _) => capturedItem = item)
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => capturedItem);

            var handler = new CreateItemCommandHandler(itemRepoMock.Object, categoryRepoMock.Object);

            var command = ValidCommand(categoryId, ownerId);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.Equal("Bike for sale", result.Title);
            Assert.Equal(100m, result.Price);
            Assert.Equal((int)ItemType.ForSale, result.ItemType);
            itemRepoMock.Verify(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_SetsOwnerAndCategoryOnCreatedItem()
        {
            var categoryId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            Item? capturedItem = null;

            var categoryRepoMock = new Mock<ICategoryRepository>();
            categoryRepoMock
                .Setup(r => r.ExistsAsync(categoryId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var itemRepoMock = new Mock<IItemRepository>();
            itemRepoMock
                .Setup(r => r.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .Callback<Item, CancellationToken>((item, _) => capturedItem = item)
                .Returns(Task.CompletedTask);
            itemRepoMock
                .Setup(r => r.GetItemWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => capturedItem);

            var handler = new CreateItemCommandHandler(itemRepoMock.Object, categoryRepoMock.Object);

            var command = ValidCommand(categoryId, ownerId);

            await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(capturedItem);
            Assert.Equal(ownerId, capturedItem!.OwnerId);
            Assert.Equal(categoryId, capturedItem.CategoryId);
            Assert.Equal(ItemStatus.Draft, capturedItem.Status);
        }
    }
}