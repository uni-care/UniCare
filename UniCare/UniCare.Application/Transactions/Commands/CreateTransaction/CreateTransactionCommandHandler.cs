using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Commands.CreateTransaction
{
    public sealed class CreateTransactionCommandHandler
          : ICommandHandler<CreateTransactionCommand, Result<CreateTransactionResult>>
    {
        private readonly ITransactionRepository _repository;

        public CreateTransactionCommandHandler(ITransactionRepository repository)
            => _repository = repository;

        public async Task<Result<CreateTransactionResult>> Handle(
            CreateTransactionCommand command,
            CancellationToken cancellationToken)
        {
            Transaction transaction;

            try
            {
                transaction = Transaction.Create(
                    itemId: command.ItemId,
                    ownerId: command.OwnerId,
                    requesterId: command.RequesterId,
                    type: command.Type,
                    agreedPrice: command.AgreedPrice,
                    rentalReturnDue: command.RentalReturnDue
                );
            }
            catch (InvalidOperationException ex)
            {
                return Result<CreateTransactionResult>.Failure(ex.Message);
            }

            await _repository.AddAsync(transaction, cancellationToken);

            return Result<CreateTransactionResult>.Success(new CreateTransactionResult(
                TransactionId: transaction.Id,
                Status: transaction.Status,
                Type: transaction.Type,
                CreatedAt: transaction.CreatedAt
            ));
        }
    }
}
