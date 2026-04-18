using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.Commands.VerifyHandover
{
    public sealed class VerifyHandoverCommandHandler
      : ICommandHandler<VerifyHandoverCommand, Result<VerifyHandoverResult>>
    {
        private readonly ITransactionHandoverRepository _repository;
        private readonly IPinGeneratorService _pinGenerator;

        public VerifyHandoverCommandHandler(
            ITransactionHandoverRepository repository,
            IPinGeneratorService pinGenerator)
        {
            _repository = repository;
            _pinGenerator = pinGenerator;
        }

        public async Task<Result<VerifyHandoverResult>> Handle(
            VerifyHandoverCommand command,
            CancellationToken cancellationToken)
        {
            var handover = await _repository.GetPendingByTransactionAndTypeAsync(
                command.TransactionId, command.Type, cancellationToken);

            if (handover is null)
                return Result<VerifyHandoverResult>.Failure(
                    "No active handover code found for this transaction.");

            if (handover.VerifiedByUserId != command.VerifyingUserId)
                return Result<VerifyHandoverResult>.Failure(
                    "You are not authorized to verify this handover.");

            // Constant-time hash comparison prevents timing attacks
            if (!_pinGenerator.VerifyPin(command.RawPin, handover.TokenHash))
                return Result<VerifyHandoverResult>.Failure(
                    "Incorrect PIN. Please try again.");

            try
            {
                handover.Verify();
                await _repository.UpdateAsync(handover, cancellationToken);

                return Result<VerifyHandoverResult>.Success(new VerifyHandoverResult(
                    Success: true,
                    Message: "Handover verified successfully.",
                    VerifiedAt: handover.VerifiedAt
                ));
            }
            catch (InvalidOperationException ex)
            {
                return Result<VerifyHandoverResult>.Failure(ex.Message);
            }
        }
    }
}
