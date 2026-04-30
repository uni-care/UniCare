using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Enums;
namespace UniCare.Application.User.commands.Auth.Register
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            When(x => x.RegistrationMethod == RegistrationMethod.Email, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("A valid email address is required.");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
            });

            When(x => x.RegistrationMethod == RegistrationMethod.Phone, () =>
            {
                RuleFor(x => x.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Phone number must be in valid E.164 format (e.g. +201012345678).");

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                    .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                    .Matches("[0-9]").WithMessage("Password must contain at least one digit.");
            });
        }
    }

}
