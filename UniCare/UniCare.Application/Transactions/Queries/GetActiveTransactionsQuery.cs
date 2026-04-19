using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Queries
{
    public sealed record GetActiveTransactionsQuery(
         Guid UserId
     ) : IQuery<Result<IReadOnlyList<ActiveTransactionResult>>>;

    public sealed record ActiveTransactionResult(
        Guid TransactionId,
        Guid ItemId,
        TransactionType Type,
        TransactionStatus Status,
        decimal AgreedPrice,
        DateTime? RentalReturnDue,
        bool IsOwner,     
        DateTime CreatedAt
    );
}
