using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.TransactionHandover.Commands.VerifyHandover
{
    public sealed class VerifyHandoverCommandValidator : AbstractValidator<VerifyHandoverCommand>
    {
        public VerifyHandoverCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid HandoverType.");

            RuleFor(x => x.VerifyingUserId)
                .NotEmpty().WithMessage("VerifyingUserId is required.");

            RuleFor(x => x.RawPin)
                .NotEmpty().WithMessage("PIN is required.")
                .Length(6).WithMessage("PIN must be exactly 6 digits.")
                .Matches(@"^\d{6}$").WithMessage("PIN must contain only digits.");
        }
    }
}
