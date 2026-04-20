using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.Commands.CreateItem
{
    public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
    {
        public CreateItemCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency must be 3 characters (e.g., USD, EUR).");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required.");

            RuleFor(x => x.AvailableTo)
                .GreaterThan(x => x.AvailableFrom)
                .When(x => x.AvailableFrom.HasValue && x.AvailableTo.HasValue)
                .WithMessage("Available To date must be after Available From date.");
        }
    }
}
