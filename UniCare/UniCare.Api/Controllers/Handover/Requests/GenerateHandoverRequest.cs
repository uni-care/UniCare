using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Api.Controllers.Handover.Requests
{
    public record GenerateHandoverRequest(
      HandoverType Type,
      Guid GeneratedForUserId,
      Guid VerifiedByUserId
  );
}
