using System;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using Xunit;

namespace UniCare.Tests.Domain
{
    public class TransactionHandoverTests
    {
        [Fact]
        public void Verify_WhenPendingAndNotExpired_SetsVerifiedStatus()
        {
            var handover = CreateHandover(expiresInMinutes: 30);

            handover.Verify();

            Assert.Equal(HandoverStatus.Verified, handover.Status);
            Assert.NotNull(handover.VerifiedAt);
        }

        [Fact]
        public void Verify_WhenExpired_SetsStatusExpiredAndThrows()
        {
            var handover = CreateHandover(expiresInMinutes: -1); // already expired

            var act = () => handover.Verify();

            Assert.Throws<InvalidOperationException>(act);
            Assert.Equal(HandoverStatus.Expired, handover.Status);
        }

        [Fact]
        public void Verify_WhenAlreadyVerified_ThrowsInvalidOperationException()
        {
            var handover = CreateHandover(expiresInMinutes: 30);
            handover.Verify();

            var act = () => handover.Verify();

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Expire_WhenPending_SetsExpiredStatus()
        {
            var handover = CreateHandover(expiresInMinutes: 30);

            handover.Expire();

            Assert.Equal(HandoverStatus.Expired, handover.Status);
        }

        [Fact]
        public void Expire_WhenAlreadyVerified_DoesNotOverwriteStatus()
        {
            var handover = CreateHandover(expiresInMinutes: 30);
            handover.Verify();

            handover.Expire();

            Assert.Equal(HandoverStatus.Verified, handover.Status);
        }

        private static TransactionHandover CreateHandover(int expiresInMinutes) =>
            TransactionHandover.Create(
                transactionId: Guid.NewGuid(),
                type: HandoverType.SaleDelivery,
                pin: "123456",
                tokenHash: "fake-hash",
                generatedForUserId: Guid.NewGuid(),
                verifiedByUserId: Guid.NewGuid(),
                expiresAt: DateTime.UtcNow.AddMinutes(expiresInMinutes));
    }
}
