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
    public sealed class GetActiveTransactionsQueryHandler
         : IQueryHandler<GetActiveTransactionsQuery, Result<IReadOnlyList<ActiveTransactionResult>>>
    {
        private readonly ITransactionRepository _repository;

        public GetActiveTransactionsQueryHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<IReadOnlyList<ActiveTransactionResult>>> Handle(
            GetActiveTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            var transactions = await _repository.GetActiveByUserAsync(query.UserId, cancellationToken);

            var results = transactions
                .Select(t => new ActiveTransactionResult(
                    TransactionId: t.Id,
                    ItemId: t.ItemId,
                    Type: t.Type,
                    Status: t.Status,
                    AgreedPrice: t.AgreedPrice,
                    RentalReturnDue: t.RentalReturnDue,
                    IsOwner: t.OwnerId == query.UserId,
                    CreatedAt: t.CreatedAt
                ))
                .ToList();

            return Result<IReadOnlyList<ActiveTransactionResult>>.Success(results);
        }
    }
}
