using FluentValidation;

namespace UniCare.Application.Chats.Queries.GetConversation
{
    public sealed class GetConversationQueryValidator : AbstractValidator<GetConversationQuery>
    {
        public GetConversationQueryValidator()
        {
            RuleFor(x => x.ChatId)
                .NotEmpty().WithMessage("ChatId is required.");

            RuleFor(x => x.RequestingUserId)
                .NotEmpty().WithMessage("RequestingUserId is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("PageNumber must be greater than 0.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100.");
        }
    }
}
