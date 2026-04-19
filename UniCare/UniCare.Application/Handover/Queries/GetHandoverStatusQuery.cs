using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Application.Handover.Queries
{
    public sealed record GetHandoverStatusQuery(
      Guid TransactionId,
      HandoverType Type
  ) : IQuery<Result<HandoverStatusResult>>;

    public sealed record HandoverStatusResult(
        Guid HandoverId,
        HandoverStatus Status,
        HandoverType Type,
        DateTime ExpiresAt,
        DateTime? VerifiedAt
    );
}
