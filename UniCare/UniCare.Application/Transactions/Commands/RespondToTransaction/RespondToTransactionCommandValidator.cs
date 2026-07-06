using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Transactions.Commands.RespondToTransaction
{
    public sealed class RespondToTransactionCommandValidator : AbstractValidator<RespondToTransactionCommand>
    {
        public RespondToTransactionCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.RespondingUserId)
                .NotEmpty().WithMessage("RespondingUserId is required.");
        }
    }
}
