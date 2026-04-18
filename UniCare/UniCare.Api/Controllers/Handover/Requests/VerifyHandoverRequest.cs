using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Api.Controllers.Handover.Requests
{
    public record VerifyHandoverRequest(
     HandoverType Type,
     Guid VerifyingUserId,
     string Pin
 );
}
