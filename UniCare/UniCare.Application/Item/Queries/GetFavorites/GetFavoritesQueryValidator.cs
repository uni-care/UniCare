using FluentValidation;

namespace UniCare.Application.Item.Queries.GetFavorites;

public class GetFavoritesQueryValidator : AbstractValidator<GetFavoritesQuery>
{
    private static readonly string[] AllowedSortByFields = ["dateAdded", "price", "name"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetFavoritesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum price must be non-negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum price must be non-negative.");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("Minimum price must not exceed maximum price.");

        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortByFields.Contains(s.ToLowerInvariant()))
            .WithMessage($"Sort by must be one of: {string.Join(", ", AllowedSortByFields)}.");

        RuleFor(x => x.SortDirection)
            .Must(d => AllowedSortDirections.Contains(d.ToLowerInvariant()))
            .WithMessage($"Sort direction must be one of: {string.Join(", ", AllowedSortDirections)}.");
    }
}
