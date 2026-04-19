using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.Transactions.Commands.CreateTransaction
{
    /// Verifies the handover PIN for the current stage of a transaction,
    /// then advances the transaction to its next status in a single atomic operation.
    public sealed record VerifyAndAdvanceTransactionCommand(
         Guid TransactionId,
         Guid VerifyingUserId,
         string RawPin
     ) : ICommand<Result<VerifyAndAdvanceResult>>;

    public sealed record VerifyAndAdvanceResult(
        bool Success,
        string Message,
        TransactionStatus NewTransactionStatus,
        HandoverType VerifiedStage,
        DateTime VerifiedAt
    );
}
