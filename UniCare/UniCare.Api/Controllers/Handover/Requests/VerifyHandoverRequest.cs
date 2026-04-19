using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Api.Controllers.Handover.Requests
{
    public record VerifyHandoverRequest(
     HandoverType Type,
     Guid VerifyingUserId,
     string Pin
 );
}
