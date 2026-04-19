using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.Queries
{
    public sealed class GetHandoverStatusQueryHandler
      : IQueryHandler<GetHandoverStatusQuery, Result<HandoverStatusResult>>
    {
        private readonly ITransactionHandoverRepository _repository;

        public GetHandoverStatusQueryHandler(ITransactionHandoverRepository repository)
            => _repository = repository;

        public async Task<Result<HandoverStatusResult>> Handle(
            GetHandoverStatusQuery query,
            CancellationToken cancellationToken)
        {
            var handover = await _repository.GetPendingByTransactionAndTypeAsync(
                query.TransactionId, query.Type, cancellationToken);

            if (handover is null)
                return Result<HandoverStatusResult>.Failure(
                    "No handover code found for this transaction and type.");

            return Result<HandoverStatusResult>.Success(new HandoverStatusResult(
                HandoverId: handover.Id,
                Status: handover.Status,
                Type: handover.Type,
                ExpiresAt: handover.ExpiresAt,
                VerifiedAt: handover.VerifiedAt
            ));
        }
    }

}
