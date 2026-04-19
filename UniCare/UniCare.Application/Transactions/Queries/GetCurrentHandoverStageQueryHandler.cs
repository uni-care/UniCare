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
    public sealed class GetCurrentHandoverStageQueryHandler
        : IQueryHandler<GetCurrentHandoverStageQuery, Result<CurrentHandoverStageResult>>
    {
        private readonly ITransactionRepository _repository;

        public GetCurrentHandoverStageQueryHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<CurrentHandoverStageResult>> Handle(
            GetCurrentHandoverStageQuery query,
            CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(query.TransactionId, cancellationToken);

            if (transaction is null)
                return Result<CurrentHandoverStageResult>.Failure(
                    "Transaction not found.");

            try
            {
                var handoverType = transaction.GetCurrentHandoverStage();

                return Result<CurrentHandoverStageResult>.Success(new CurrentHandoverStageResult(
                    TransactionId: transaction.Id,
                    TransactionStatus: transaction.Status,
                    TransactionType: transaction.Type,
                    NextHandoverType: handoverType,
                    OwnerId: transaction.OwnerId,
                    RequesterId: transaction.RequesterId
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Result<CurrentHandoverStageResult>.Failure(ex.Message);
            }
        }
    }
}
