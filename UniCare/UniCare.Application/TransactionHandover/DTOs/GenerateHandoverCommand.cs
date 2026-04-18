
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.DTOs
{
    public record GenerateHandoverCommand(
      Guid TransactionId,
      HandoverType Type,
      Guid GeneratedForUserId,   // User who will present/enter the code (receiver)
      Guid VerifiedByUserId      // User who confirms (the other party)
  );
}
