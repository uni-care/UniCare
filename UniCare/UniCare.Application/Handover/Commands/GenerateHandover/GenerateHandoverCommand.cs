using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.Commands.GenerateHandover
{
    public record GenerateHandoverCommand(
      Guid TransactionId,
      HandoverType Type,
      Guid GeneratedForUserId,   // User who will present/enter the code (receiver)
      Guid VerifiedByUserId      // User who confirms (the other party)
  ) : ICommand<Result<GenerateHandoverResult>>;
    public sealed record GenerateHandoverResult(
    Guid HandoverId,
    string Pin,         // Raw 6-digit PIN shown once to the receiver
    string QrPayload,   // JSON the mobile app encodes into a QR image
    HandoverType Type,
    DateTime ExpiresAt
);
}
