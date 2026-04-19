using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Domain.Aggregates.TransactionAggregate
{
    public class Transaction
    {
        public Guid Id { get; private set; }

        public Guid ItemId { get; private set; }

        public Guid OwnerId { get; private set; }

        /// The student who wants to buy or rent.
        public Guid RequesterId { get; private set; }

        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }

        public decimal AgreedPrice { get; private set; }

        public DateTime? RentalReturnDue { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private Transaction() { } 

        public static Transaction Create(
            Guid itemId,
            Guid ownerId,
            Guid requesterId,
            TransactionType type,
            decimal agreedPrice,
            DateTime? rentalReturnDue = null)
        {
            if (ownerId == requesterId)
                throw new InvalidOperationException("Owner and requester cannot be the same user.");

            if (type == TransactionType.Rental && rentalReturnDue is null)
                throw new InvalidOperationException("Rental transactions must have a return due date.");

            return new Transaction
            {
                Id = Guid.NewGuid(),
                ItemId = itemId,
                OwnerId = ownerId,
                RequesterId = requesterId,
                Type = type,
                Status = TransactionStatus.PendingApproval,
                AgreedPrice = agreedPrice,
                RentalReturnDue = rentalReturnDue,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void Approve()
        {
            EnsureStatus(TransactionStatus.PendingApproval, "approve");
            Status = TransactionStatus.AwaitingHandover;
            UpdatedAt = DateTime.UtcNow;
        }

        /// Called after the SaleDelivery or RentalStart handover is verified.
        public void MarkActive()
        {
            EnsureStatus(TransactionStatus.AwaitingHandover, "mark as active");
            Status = TransactionStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        /// Called after the final handover is verified:
        ///   - Sale:   after SaleDelivery (MarkActive → Complete in one verify call)
        ///   - Rental: after RentalReturn
        public void Complete()
        {
            EnsureStatus(TransactionStatus.Active, "complete");
            Status = TransactionStatus.Completed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status is TransactionStatus.Completed or TransactionStatus.Cancelled)
                throw new InvalidOperationException($"Cannot cancel a {Status} transaction.");

            Status = TransactionStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        /// Determines which HandoverType should be generated next based on
        /// the transaction's current status and type.
        /// Called by both GET /code and POST /verify-code so they stay in sync.
        public HandoverType GetCurrentHandoverStage()
        {
            return (Type, Status) switch
            {
                (TransactionType.Sale, TransactionStatus.AwaitingHandover) => HandoverType.SaleDelivery,
                (TransactionType.Rental, TransactionStatus.AwaitingHandover) => HandoverType.RentalStart,
                (TransactionType.Rental, TransactionStatus.Active) => HandoverType.RentalReturn,
                _ => throw new InvalidOperationException(
                    $"No handover stage available for a {Type} transaction in '{Status}' status.")
            };
        }

        private void EnsureStatus(TransactionStatus expected, string action)
        {
            if (Status != expected)
                throw new InvalidOperationException(
                    $"Cannot {action} a transaction that is in '{Status}' status.");
        }
    }
}
