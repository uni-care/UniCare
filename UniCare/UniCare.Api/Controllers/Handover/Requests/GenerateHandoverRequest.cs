using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Api.Controllers.Handover.Requests
{
    public record GenerateHandoverRequest(
      HandoverType Type,
      Guid GeneratedForUserId,
      Guid VerifiedByUserId
  );
}
