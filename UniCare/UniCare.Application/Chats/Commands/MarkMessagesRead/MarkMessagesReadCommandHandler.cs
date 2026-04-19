using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chats.Commands.MarkMessagesRead
{
    public sealed class MarkMessagesReadCommandHandler
       : ICommandHandler<MarkMessagesReadCommand, Result>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _notificationService;

        public MarkMessagesReadCommandHandler(
            IChatRepository chatRepository,
            IChatNotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(
            MarkMessagesReadCommand command,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken);

            if (chat is null)
                return Result.Failure("Chat not found.");

            try
            {
                chat.EnsureParticipant(command.ReaderId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _chatRepository.MarkMessagesReadAsync(
                command.ChatId, command.ReaderId, cancellationToken);

            await _notificationService.NotifyMessagesReadAsync(
                command.ChatId, command.ReaderId, cancellationToken);

            return Result.Success();
        }
    }
}
