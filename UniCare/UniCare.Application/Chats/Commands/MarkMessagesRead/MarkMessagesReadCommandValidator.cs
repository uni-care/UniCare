using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Chats.Commands.MarkMessagesRead
{
    public sealed class MarkMessagesReadCommandValidator : AbstractValidator<MarkMessagesReadCommand>
    {
        public MarkMessagesReadCommandValidator()
        {
            RuleFor(x => x.ChatId)
                .NotEmpty().WithMessage("ChatId is required.");

            RuleFor(x => x.ReaderId)
                .NotEmpty().WithMessage("ReaderId is required.");
        }
    }
}
