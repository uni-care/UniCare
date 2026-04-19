using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Chats.Commands.GetOrCreateChat
{
    public sealed class GetOrCreateChatCommandValidator : AbstractValidator<GetOrCreateChatCommand>
    {
        public GetOrCreateChatCommandValidator()
        {
            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("TransactionId is required.");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("OwnerId is required.");

            RuleFor(x => x.RequesterId)
                .NotEmpty().WithMessage("RequesterId is required.");

            RuleFor(x => x)
                .Must(x => x.OwnerId != x.RequesterId)
                .WithMessage("Owner and requester cannot be the same user.");
        }
    }
}
