using FluentValidation;

namespace UniCare.Application.Item.Queries.GetFavorites;

public class GetFavoritesQueryValidator : AbstractValidator<GetFavoritesQuery>
{
    private static readonly string[] AllowedSortByFields = ["dateAdded", "price", "name"];
    private static readonly string[] AllowedSortDirections = ["asc", "desc"];

    public GetFavoritesQueryValidator()
    {
        RuleFor(x => x.UserId)
        .NotEmpty()
        .WithMessage("User id is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(s => AllowedSortByFields.Contains(s))
            .WithMessage($"Sort by must be one of: {string.Join(", ", AllowedSortByFields)}.");

        RuleFor(x => x.SortDirection)
            .Must(d => AllowedSortDirections.Contains(d.ToLowerInvariant()))
            .WithMessage($"Sort direction must be one of: {string.Join(", ", AllowedSortDirections)}.");
    }
}
