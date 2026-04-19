using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Application.Transactions.Commands.CreateTransaction
{
   
    public sealed class VerifyAndAdvanceTransactionCommandHandler
        : ICommandHandler<VerifyAndAdvanceTransactionCommand, Result<VerifyAndAdvanceResult>>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly ITransactionHandoverRepository _handoverRepo;
        private readonly IPinGeneratorService _pinGenerator;

        public VerifyAndAdvanceTransactionCommandHandler(
            ITransactionRepository transactionRepo,
            ITransactionHandoverRepository handoverRepo,
            IPinGeneratorService pinGenerator)
        {
            _transactionRepo = transactionRepo;
            _handoverRepo = handoverRepo;
            _pinGenerator = pinGenerator;
        }

        public async Task<Result<VerifyAndAdvanceResult>> Handle(
            VerifyAndAdvanceTransactionCommand command,
            CancellationToken cancellationToken)
        {
            var transaction = await _transactionRepo.GetByIdAsync(
                command.TransactionId, cancellationToken);

            if (transaction is null)
                return Result<VerifyAndAdvanceResult>.Failure("Transaction not found.");

            HandoverType currentStage;
            try
            {
                currentStage = transaction.GetCurrentHandoverStage();
            }
            catch (InvalidOperationException ex)
            {
                return Result<VerifyAndAdvanceResult>.Failure(ex.Message);
            }

            var handover = await _handoverRepo.GetPendingByTransactionAndTypeAsync(
                command.TransactionId, currentStage, cancellationToken);

            if (handover is null)
                return Result<VerifyAndAdvanceResult>.Failure(
                    "No active handover code found. Please generate a code first.");

            if (handover.VerifiedByUserId != command.VerifyingUserId)
                return Result<VerifyAndAdvanceResult>.Failure(
                    "You are not authorized to verify this handover.");

            if (!_pinGenerator.VerifyPin(command.RawPin, handover.TokenHash))
                return Result<VerifyAndAdvanceResult>.Failure(
                    "Incorrect PIN. Please try again.");

            try
            {
                handover.Verify(); 

                switch (currentStage)
                {
                    case HandoverType.SaleDelivery:
                     
                        transaction.MarkActive();
                        transaction.Complete();
                        break;

                    case HandoverType.RentalStart:
                      
                        transaction.MarkActive();
                        break;

                    case HandoverType.RentalReturn:
                     
                        transaction.Complete();
                        break;
                }

                await _handoverRepo.UpdateAsync(handover, cancellationToken);
                await _transactionRepo.UpdateAsync(transaction, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return Result<VerifyAndAdvanceResult>.Failure(ex.Message);
            }

            return Result<VerifyAndAdvanceResult>.Success(new VerifyAndAdvanceResult(
                Success: true,
                Message: $"{currentStage} verified. Transaction is now {transaction.Status}.",
                NewTransactionStatus: transaction.Status,
                VerifiedStage: currentStage,
                VerifiedAt: handover.VerifiedAt!.Value
            ));
        }
    }
}
