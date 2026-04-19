using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Application.Handover.Commands.VerifyHandover
{
    public sealed record VerifyHandoverCommand(
     Guid TransactionId,
     HandoverType Type,
     Guid VerifyingUserId,
     string RawPin           // 6 digits typed manually OR decoded from QR scan
 ) : ICommand<Result<VerifyHandoverResult>>;

    public sealed record VerifyHandoverResult(
        bool Success,
        string Message,
        DateTime? VerifiedAt
    );

}
