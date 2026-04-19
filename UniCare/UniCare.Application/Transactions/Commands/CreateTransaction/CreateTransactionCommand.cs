using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Commands.CreateTransaction
{
    public sealed record CreateTransactionCommand(
           Guid ItemId,
           Guid OwnerId,
           Guid RequesterId,
           TransactionType Type,
           decimal AgreedPrice,
           DateTime? RentalReturnDue   // required when Type == Rental
       ) : ICommand<Result<CreateTransactionResult>>;

    public sealed record CreateTransactionResult(
        Guid TransactionId,
        TransactionStatus Status,
        TransactionType Type,
        DateTime CreatedAt
    );
}
