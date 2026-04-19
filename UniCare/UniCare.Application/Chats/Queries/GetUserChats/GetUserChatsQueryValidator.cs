using FluentValidation;

namespace UniCare.Application.Chats.Queries.GetUserChats
{
    public sealed class GetUserChatsQueryValidator : AbstractValidator<GetUserChatsQuery>
    {
        public GetUserChatsQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
