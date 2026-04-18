using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Transactions.Commands.CreateTransaction
{
    public sealed class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("ItemId is required.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.");

            RuleFor(x => x.RequesterId)
                .NotEmpty().WithMessage("RequesterId is required.");

            RuleFor(x => x)
                .Must(x => x.OwnerId != x.RequesterId)
                .WithMessage("Owner and requester cannot be the same user.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be Sale or Rental.");

            RuleFor(x => x.AgreedPrice)
                .GreaterThan(0).WithMessage("AgreedPrice must be greater than zero.");

            // RentalReturnDue is mandatory for rentals and must be in the future
            RuleFor(x => x.RentalReturnDue)
                .NotNull().WithMessage("RentalReturnDue is required for rental transactions.")
                .GreaterThan(DateTime.UtcNow).WithMessage("RentalReturnDue must be in the future.")
                .When(x => x.Type == TransactionType.Rental);
        }
    }
}
