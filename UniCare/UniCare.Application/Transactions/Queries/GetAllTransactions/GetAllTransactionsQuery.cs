using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Application.Transactions.Queries.GetActiveTransactions;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Queries.GetAllTransactions
{
    public sealed record GetAllTransactionsQuery(
       Guid UserId
   ) : IQuery<Result<IReadOnlyList<AllTransactionsResult>>>;

    public sealed record AllTransactionsResult(
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
