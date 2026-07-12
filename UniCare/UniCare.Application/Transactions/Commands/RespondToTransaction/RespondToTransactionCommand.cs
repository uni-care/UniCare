using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Commands.RespondToTransaction
{
    
    public sealed record RespondToTransactionCommand(
        Guid TransactionId,
        Guid RespondingUserId,   // must be the item owner
        bool IsApproved
    ) : ICommand<Result<RespondToTransactionResult>>;

    public sealed record RespondToTransactionResult(
        Guid TransactionId,
        TransactionStatus Status,
        DateTime UpdatedAt
    );
}
