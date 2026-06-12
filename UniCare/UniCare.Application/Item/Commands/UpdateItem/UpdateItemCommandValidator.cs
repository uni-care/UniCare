using FluentValidation;
using UniCare.Domain.Enums;

namespace UniCare.Application.Item.Commands.UpdateItem
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        private static readonly string[] ValidStatuses = Enum.GetNames<ItemStatus>();

        public UpdateItemCommandValidator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty()
                .WithMessage("Item ID is required.");

            RuleFor(x => x.RequestingUserId)
                .NotEmpty()
                .WithMessage("Requesting user ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Title cannot be empty when provided.")
                .MaximumLength(200)
                .WithMessage("Title must not exceed 200 characters.")
                .When(x => x.Title is not null);

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty when provided.")
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.")
                .When(x => x.Description is not null);

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0.")
                .LessThanOrEqualTo(1_000_000)
                .WithMessage("Price must not exceed 1,000,000.")
                .When(x => x.Price.HasValue);

            RuleFor(x => x)
                .Must(x => x.Price.HasValue && !string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage("Currency is required when providing a Price.")
                .When(x => x.Price.HasValue && string.IsNullOrWhiteSpace(x.Currency))
                .OverridePropertyName("Currency");

            RuleFor(x => x)
                .Must(x => !x.Price.HasValue && string.IsNullOrWhiteSpace(x.Currency))
                .WithMessage("Price is required when providing a Currency.")
                .When(x => !x.Price.HasValue && !string.IsNullOrWhiteSpace(x.Currency))
                .OverridePropertyName("Price");

            RuleFor(x => x.Currency)
                .Length(3)
                .WithMessage("Currency must be a 3-letter ISO code (e.g., USD, EUR, EGP).")
                .When(x => !string.IsNullOrWhiteSpace(x.Currency));

            RuleFor(x => x.Status)
                .Must(s => ValidStatuses.Contains(s, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}.")
                .When(x => !string.IsNullOrEmpty(x.Status));

            RuleForEach(x => x.ImageUrls)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                             (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Each image URL must be a valid absolute HTTP or HTTPS URL.")
                .When(x => x.ImageUrls is { Count: > 0 });

            RuleFor(x => x.AvailableTo)
                .GreaterThan(x => x.AvailableFrom)
                .WithMessage("Available To date must be after Available From date.")
                .When(x => x.AvailableFrom.HasValue && x.AvailableTo.HasValue);
        }
    }
}
