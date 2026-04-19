using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Chats.Commands.SendMessage
{
    public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.ChatId)
                .NotEmpty().WithMessage("ChatId is required.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("SenderId is required.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Message body cannot be empty.")
                .MaximumLength(2000).WithMessage("Message body cannot exceed 2000 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid message type.");
        }
    }
}
