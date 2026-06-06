using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.Commands.UpdateItem
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item ID is required.");

            RuleFor(x => x.Title)
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.")
                .When(x => x.Price.HasValue);

            RuleFor(x => x.Currency)
                .Length(3).WithMessage("Currency must be 3 characters.")
                .When(x => !string.IsNullOrEmpty(x.Currency));
        }
    }
}
