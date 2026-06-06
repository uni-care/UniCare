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
    public sealed class GetAllTransactionsQueryHandler
          : IQueryHandler<GetAllTransactionsQuery, Result<IReadOnlyList<AllTransactionsResult>>>
    {
        private readonly ITransactionRepository _repository;

        public GetAllTransactionsQueryHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<IReadOnlyList<AllTransactionsResult>>> Handle(
            GetAllTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            var transactions = await _repository.GetByUserAsync(query.UserId, cancellationToken);

            var result = transactions
                .Select(t => new AllTransactionsResult(
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

            return Result<IReadOnlyList<AllTransactionsResult>>.Success(result);
        }
    }
}
