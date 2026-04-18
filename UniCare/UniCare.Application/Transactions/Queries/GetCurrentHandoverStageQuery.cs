using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.Transactions.Queries
{
    public sealed record GetCurrentHandoverStageQuery(
         Guid TransactionId
     ) : IQuery<Result<CurrentHandoverStageResult>>;

    public sealed record CurrentHandoverStageResult(
        Guid TransactionId,
        TransactionStatus TransactionStatus,
        TransactionType TransactionType,
        HandoverType NextHandoverType,   
        Guid OwnerId,
        Guid RequesterId
    );
}
