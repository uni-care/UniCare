using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Transactions.Commands.CreateTransaction;
using UniCare.Domain.Aggregates.TransactionAggregate;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers
{
    public class CreateTransactionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ValidSaleCommand_ReturnsSuccessAndPersists()
        {
            var repoMock = new Mock<ITransactionRepository>();
            var handler = new CreateTransactionCommandHandler(repoMock.Object);

            var command = new CreateTransactionCommand(
                ItemId: Guid.NewGuid(),
                OwnerId: Guid.NewGuid(),
                RequesterId: Guid.NewGuid(),
                Type: TransactionType.Sale,
                AgreedPrice: 100m,
                RentalReturnDue: null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(TransactionStatus.PendingApproval, result.Data!.Status);
            Assert.Equal(TransactionType.Sale, result.Data.Type);

            repoMock.Verify(r => r.AddAsync(
                It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidRentalCommand_ReturnsSuccess()
        {
            var repoMock = new Mock<ITransactionRepository>();
            var handler = new CreateTransactionCommandHandler(repoMock.Object);

            var command = new CreateTransactionCommand(
                ItemId: Guid.NewGuid(),
                OwnerId: Guid.NewGuid(),
                RequesterId: Guid.NewGuid(),
                Type: TransactionType.Rental,
                AgreedPrice: 20m,
                RentalReturnDue: DateTime.UtcNow.AddDays(7));

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(TransactionType.Rental, result.Data!.Type);
        }

        [Fact]
        public async Task Handle_SameOwnerAndRequester_ReturnsFailureAndDoesNotPersist()
        {
            var repoMock = new Mock<ITransactionRepository>();
            var handler = new CreateTransactionCommandHandler(repoMock.Object);

            var userId = Guid.NewGuid();
            var command = new CreateTransactionCommand(
                Guid.NewGuid(), userId, userId, TransactionType.Sale, 100m, null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            repoMock.Verify(r => r.AddAsync(
                It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RentalWithoutReturnDue_ReturnsFailureAndDoesNotPersist()
        {
            // Note: this scenario should normally be caught by CreateTransactionCommandValidator
            // in the pipeline before it ever reaches the handler. This test verifies the
            // handler's own defensive check (Transaction.Create's domain guard) still holds.
            var repoMock = new Mock<ITransactionRepository>();
            var handler = new CreateTransactionCommandHandler(repoMock.Object);

            var command = new CreateTransactionCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                TransactionType.Rental, 100m, RentalReturnDue: null);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            repoMock.Verify(r => r.AddAsync(
                It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
