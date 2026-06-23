using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Loans.Queries.GetLoans
{
    public sealed class GetLoansQueryValidator : AbstractValidator<GetLoansQuery>
    {
        public GetLoansQueryValidator()
        {
            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");

            RuleFor(x => x.StatusFilter)
                .IsInEnum().WithMessage("Invalid status filter value.")
                .When(x => x.StatusFilter.HasValue);

            RuleFor(x => x.SortBy)
                .IsInEnum().WithMessage("Invalid sortBy value.");

            // If both date bounds are provided, from must be before to
            RuleFor(x => x.LoanedFrom)
                .LessThan(x => x.LoanedTo)
                .When(x => x.LoanedFrom.HasValue && x.LoanedTo.HasValue)
                .WithMessage("LoanedFrom must be earlier than LoanedTo.");

            RuleFor(x => x.ReturnDueFrom)
                .LessThan(x => x.ReturnDueTo)
                .When(x => x.ReturnDueFrom.HasValue && x.ReturnDueTo.HasValue)
                .WithMessage("ReturnDueFrom must be earlier than ReturnDueTo.");
        }
    }
}
