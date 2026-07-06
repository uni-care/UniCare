using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Commands.RespondToTransaction
{

    public sealed class RespondToTransactionCommandHandler
        : ICommandHandler<RespondToTransactionCommand, Result<RespondToTransactionResult>>
    {
        private readonly ITransactionRepository _repository;

        public RespondToTransactionCommandHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<RespondToTransactionResult>> Handle(
            RespondToTransactionCommand command,
            CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(command.TransactionId, cancellationToken);

            if (transaction is null)
                return Result<RespondToTransactionResult>.NotFound("Transaction not found.");

            if (transaction.OwnerId != command.RespondingUserId)
                return Result<RespondToTransactionResult>.Forbidden(
                    "Only the item owner can approve or decline this transaction.");

            if (transaction.Status != TransactionStatus.PendingApproval)
                return Result<RespondToTransactionResult>.Conflict(
                    $"Cannot respond to a transaction that is in '{transaction.Status}' status.");

            try
            {
                if (command.IsApproved)
                    transaction.Approve();
                else
                    transaction.Cancel();
            }
            catch (InvalidOperationException ex)
            {
                return Result<RespondToTransactionResult>.Failure(ex.Message);
            }

            await _repository.UpdateAsync(transaction, cancellationToken);

            return Result<RespondToTransactionResult>.Success(new RespondToTransactionResult(
                TransactionId: transaction.Id,
                Status: transaction.Status,
                UpdatedAt: transaction.UpdatedAt
            ));
        }
    }
}
