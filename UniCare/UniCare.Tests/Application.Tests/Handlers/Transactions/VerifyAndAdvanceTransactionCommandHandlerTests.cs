using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Transactions.Commands.CreateTransaction;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers.Transactions
{
    public class VerifyAndAdvanceTransactionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_TransactionNotFound_ReturnsFailure()
        {
            var transactionRepoMock = new Mock<ITransactionRepository>();
            transactionRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object,
                Mock.Of<ITransactionHandoverRepository>(),
                Mock.Of<IPinGeneratorService>());

            var command = new VerifyAndAdvanceTransactionCommand(Guid.NewGuid(), Guid.NewGuid(), "123456");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_SaleDelivery_CompletesTransactionInOneStep()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(Guid.NewGuid(), ownerId, requesterId, TransactionType.Sale, 100m);
            transaction.Approve(); // PendingApproval -> AwaitingHandover

            var handover = TransactionHandover.Create(
                transactionId: transaction.Id,
                type: HandoverType.SaleDelivery,
                pin: "123456",
                tokenHash: "irrelevant-because-pin-check-is-mocked",
                generatedForUserId: requesterId,
                verifiedByUserId: ownerId,
                expiresAt: DateTime.UtcNow.AddMinutes(30));

            var (transactionRepoMock, handoverRepoMock, pinGeneratorMock) =
                BuildMocks(transaction, HandoverType.SaleDelivery, handover, pinIsValid: true);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, pinGeneratorMock.Object);

            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, ownerId, "123456");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(HandoverType.SaleDelivery, result.Data!.VerifiedStage);
            Assert.Equal(TransactionStatus.Completed, result.Data.NewTransactionStatus);
            transactionRepoMock.Verify(r => r.UpdateAsync(transaction, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RentalStart_MovesTransactionToActiveOnly()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(
                Guid.NewGuid(), ownerId, requesterId, TransactionType.Rental, 30m,
                DateTime.UtcNow.AddDays(5));
            transaction.Approve(); // -> AwaitingHandover

            var handover = TransactionHandover.Create(
                transaction.Id, HandoverType.RentalStart, "654321", "hash",
                generatedForUserId: requesterId, verifiedByUserId: ownerId,
                expiresAt: DateTime.UtcNow.AddMinutes(30));

            var (transactionRepoMock, handoverRepoMock, pinGeneratorMock) =
                BuildMocks(transaction, HandoverType.RentalStart, handover, pinIsValid: true);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, pinGeneratorMock.Object);

            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, ownerId, "654321");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(TransactionStatus.Active, result.Data!.NewTransactionStatus);
        }

        [Fact]
        public async Task Handle_RentalReturn_CompletesActiveRental()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(
                Guid.NewGuid(), ownerId, requesterId, TransactionType.Rental, 30m,
                DateTime.UtcNow.AddDays(5));
            transaction.Approve();
            transaction.MarkActive(); // -> Active, awaiting RentalReturn

            var handover = TransactionHandover.Create(
                transaction.Id, HandoverType.RentalReturn, "111222", "hash",
                generatedForUserId: ownerId, verifiedByUserId: requesterId,
                expiresAt: DateTime.UtcNow.AddMinutes(30));

            var (transactionRepoMock, handoverRepoMock, pinGeneratorMock) =
                BuildMocks(transaction, HandoverType.RentalReturn, handover, pinIsValid: true);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, pinGeneratorMock.Object);

            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, requesterId, "111222");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(TransactionStatus.Completed, result.Data!.NewTransactionStatus);
        }

        [Fact]
        public async Task Handle_IncorrectPin_ReturnsFailureAndDoesNotAdvanceTransaction()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(Guid.NewGuid(), ownerId, requesterId, TransactionType.Sale, 100m);
            transaction.Approve();

            var handover = TransactionHandover.Create(
                transaction.Id, HandoverType.SaleDelivery, "123456", "hash",
                generatedForUserId: requesterId, verifiedByUserId: ownerId,
                expiresAt: DateTime.UtcNow.AddMinutes(30));

            var (transactionRepoMock, handoverRepoMock, pinGeneratorMock) =
                BuildMocks(transaction, HandoverType.SaleDelivery, handover, pinIsValid: false);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, pinGeneratorMock.Object);

            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, ownerId, "000000");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal(TransactionStatus.AwaitingHandover, transaction.Status); // unchanged
            transactionRepoMock.Verify(r => r.UpdateAsync(
                It.IsAny<Transaction>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WrongVerifyingUser_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(Guid.NewGuid(), ownerId, requesterId, TransactionType.Sale, 100m);
            transaction.Approve();

            var handover = TransactionHandover.Create(
                transaction.Id, HandoverType.SaleDelivery, "123456", "hash",
                generatedForUserId: requesterId, verifiedByUserId: ownerId, // only owner may verify
                expiresAt: DateTime.UtcNow.AddMinutes(30));

            var (transactionRepoMock, handoverRepoMock, pinGeneratorMock) =
                BuildMocks(transaction, HandoverType.SaleDelivery, handover, pinIsValid: true);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, pinGeneratorMock.Object);

            // A random third party attempts to verify instead of the owner.
            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, Guid.NewGuid(), "123456");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_NoActiveHandoverCode_ReturnsFailure()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var transaction = Transaction.Create(Guid.NewGuid(), ownerId, requesterId, TransactionType.Sale, 100m);
            transaction.Approve();

            var transactionRepoMock = new Mock<ITransactionRepository>();
            transactionRepoMock
                .Setup(r => r.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            var handoverRepoMock = new Mock<ITransactionHandoverRepository>();
            handoverRepoMock
                .Setup(r => r.GetPendingByTransactionAndTypeAsync(
                    transaction.Id, HandoverType.SaleDelivery, It.IsAny<CancellationToken>()))
                .ReturnsAsync((TransactionHandover?)null);

            var handler = new VerifyAndAdvanceTransactionCommandHandler(
                transactionRepoMock.Object, handoverRepoMock.Object, Mock.Of<IPinGeneratorService>());

            var command = new VerifyAndAdvanceTransactionCommand(transaction.Id, ownerId, "123456");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }

        private static (Mock<ITransactionRepository>, Mock<ITransactionHandoverRepository>, Mock<IPinGeneratorService>)
            BuildMocks(Transaction transaction, HandoverType stage, TransactionHandover handover, bool pinIsValid)
        {
            var transactionRepoMock = new Mock<ITransactionRepository>();
            transactionRepoMock
                .Setup(r => r.GetByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(transaction);

            var handoverRepoMock = new Mock<ITransactionHandoverRepository>();
            handoverRepoMock
                .Setup(r => r.GetPendingByTransactionAndTypeAsync(transaction.Id, stage, It.IsAny<CancellationToken>()))
                .ReturnsAsync(handover);

            var pinGeneratorMock = new Mock<IPinGeneratorService>();
            pinGeneratorMock
                .Setup(p => p.VerifyPin(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(pinIsValid);

            return (transactionRepoMock, handoverRepoMock, pinGeneratorMock);
        }
    }
}
