using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionHandoverAggregate
{
    public class TransactionHandover
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public HandoverType Type { get; private set; }           // SaleDelivery | RentalStart | RentalReturn
        public HandoverStatus Status { get; private set; }       // Pending | Verified | Expired
        public string TokenHash { get; private set; } = default!; // SHA-256 of the raw PIN
        public string Pin { get; private set; } = default!;      // 6-digit PIN (only returned once at creation)
        public DateTime ExpiresAt { get; private set; }
        public DateTime? VerifiedAt { get; private set; }
        public Guid GeneratedForUserId { get; private set; }     // Who should enter this code
        public Guid VerifiedByUserId { get; private set; }       // Who confirms receipt (other party)

        private TransactionHandover() { }

        public static TransactionHandover Create(
            Guid transactionId,
            HandoverType type,
            string pin,
            string tokenHash,
            Guid generatedForUserId,
            Guid verifiedByUserId,
            DateTime expiresAt)
        {
            return new TransactionHandover
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                Type = type,
                Status = HandoverStatus.Pending,
                Pin = pin,
                TokenHash = tokenHash,
                GeneratedForUserId = generatedForUserId,
                VerifiedByUserId = verifiedByUserId,
                ExpiresAt = expiresAt
            };
        }

        public void Verify()
        {
            if (Status != HandoverStatus.Pending)
                throw new InvalidOperationException("Handover code has already been used or expired.");

            if (DateTime.UtcNow > ExpiresAt)
            {
                Status = HandoverStatus.Expired;
                throw new InvalidOperationException("Handover code has expired.");
            }

            Status = HandoverStatus.Verified;
            VerifiedAt = DateTime.UtcNow;
        }

        public void Expire()
        {
            if (Status == HandoverStatus.Pending)
                Status = HandoverStatus.Expired;
        }
    }
}
