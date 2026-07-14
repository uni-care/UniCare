using System;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using Xunit;

namespace UniCare.Tests.Domain
{
    
    public class TransactionTests
    {
        [Fact]
        public void Create_WithSameOwnerAndRequester_ThrowsInvalidOperationException()
        {
            var userId = Guid.NewGuid();

            var act = () => Transaction.Create(
                itemId: Guid.NewGuid(),
                ownerId: userId,
                requesterId: userId,
                type: TransactionType.Sale,
                agreedPrice: 100m);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Create_RentalWithoutReturnDue_ThrowsInvalidOperationException()
        {
            var act = () => Transaction.Create(
                itemId: Guid.NewGuid(),
                ownerId: Guid.NewGuid(),
                requesterId: Guid.NewGuid(),
                type: TransactionType.Rental,
                agreedPrice: 100m,
                rentalReturnDue: null);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Create_ValidRental_SetsPendingApprovalStatus()
        {
            var transaction = Transaction.Create(
                itemId: Guid.NewGuid(),
                ownerId: Guid.NewGuid(),
                requesterId: Guid.NewGuid(),
                type: TransactionType.Rental,
                agreedPrice: 50m,
                rentalReturnDue: DateTime.UtcNow.AddDays(7));

            Assert.Equal(TransactionStatus.PendingApproval, transaction.Status);
        }

        [Fact]
        public void Approve_FromPendingApproval_MovesToAwaitingHandover()
        {
            var transaction = CreateSaleTransaction();

            transaction.Approve();

            Assert.Equal(TransactionStatus.AwaitingHandover, transaction.Status);
        }

        [Fact]
        public void Approve_WhenNotPendingApproval_ThrowsInvalidOperationException()
        {
            var transaction = CreateSaleTransaction();
            transaction.Approve(); // now AwaitingHandover

            Assert.Throws<InvalidOperationException>(() => transaction.Approve());
        }

        [Fact]
        public void Complete_WhenNotActive_ThrowsInvalidOperationException()
        {
            var transaction = CreateSaleTransaction();
            // Status is still PendingApproval here.

            Assert.Throws<InvalidOperationException>(() => transaction.Complete());
        }

        [Fact]
        public void MarkActive_ThenComplete_ResultsInCompletedStatus()
        {
            var transaction = CreateSaleTransaction();
            transaction.Approve();
            transaction.MarkActive();
            transaction.Complete();

            Assert.Equal(TransactionStatus.Completed, transaction.Status);
        }

        [Fact]
        public void Cancel_WhenAlreadyCompleted_ThrowsInvalidOperationException()
        {
            var transaction = CreateSaleTransaction();
            transaction.Approve();
            transaction.MarkActive();
            transaction.Complete();

            Assert.Throws<InvalidOperationException>(() => transaction.Cancel());
        }

        [Fact]
        public void Cancel_FromPendingApproval_SetsCancelledStatus()
        {
            var transaction = CreateSaleTransaction();

            transaction.Cancel();

            Assert.Equal(TransactionStatus.Cancelled, transaction.Status);
        }

        [Theory]
        [InlineData(TransactionType.Sale, TransactionStatus.AwaitingHandover, HandoverType.SaleDelivery)]
        [InlineData(TransactionType.Rental, TransactionStatus.AwaitingHandover, HandoverType.RentalStart)]
        [InlineData(TransactionType.Rental, TransactionStatus.Active, HandoverType.RentalReturn)]
        public void GetCurrentHandoverStage_ReturnsExpectedStage(
            TransactionType type, TransactionStatus targetStatus, HandoverType expected)
        {
            var transaction = type == TransactionType.Rental
                ? Transaction.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    type, 100m, DateTime.UtcNow.AddDays(3))
                : Transaction.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                    type, 100m);

            transaction.Approve(); // -> AwaitingHandover
            if (targetStatus == TransactionStatus.Active)
                transaction.MarkActive(); // -> Active (for rental return case)

            var stage = transaction.GetCurrentHandoverStage();

            Assert.Equal(expected, stage);
        }

        [Fact]
        public void GetCurrentHandoverStage_WhenCompleted_ThrowsInvalidOperationException()
        {
            var transaction = CreateSaleTransaction();
            transaction.Approve();
            transaction.MarkActive();
            transaction.Complete();

            Assert.Throws<InvalidOperationException>(() => transaction.GetCurrentHandoverStage());
        }

        private static Transaction CreateSaleTransaction() =>
            Transaction.Create(
                itemId: Guid.NewGuid(),
                ownerId: Guid.NewGuid(),
                requesterId: Guid.NewGuid(),
                type: TransactionType.Sale,
                agreedPrice: 100m);
    }
}
