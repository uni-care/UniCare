using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.TransactionHandover.Commands.GenerateHandover
{
    public sealed class GenerateHandoverCommandValidator : AbstractValidator<GenerateHandoverCommand>
    {
        public GenerateHandoverCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("HandoverType must be SaleDelivery, RentalStart, or RentalReturn.");

            RuleFor(x => x.GeneratedForUserId)
                .NotEmpty().WithMessage("GeneratedForUserId is required.");

            RuleFor(x => x.VerifiedByUserId)
                .NotEmpty().WithMessage("VerifiedByUserId is required.");

            RuleFor(x => x)
                .Must(x => x.GeneratedForUserId != x.VerifiedByUserId)
                .WithMessage("The code receiver and verifier cannot be the same user.");
        }
    }

}
