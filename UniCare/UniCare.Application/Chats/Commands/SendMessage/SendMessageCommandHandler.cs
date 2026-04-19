using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chats.Commands.SendMessage
{
    public sealed class SendMessageCommandHandler
        : ICommandHandler<SendMessageCommand, Result<SendMessageResult>>
    {
        private readonly IChatRepository _chatRepository;
        private readonly IChatNotificationService _notificationService;

        public SendMessageCommandHandler(
            IChatRepository chatRepository,
            IChatNotificationService notificationService)
        {
            _chatRepository = chatRepository;
            _notificationService = notificationService;
        }

        public async Task<Result<SendMessageResult>> Handle(
            SendMessageCommand command,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(command.ChatId, cancellationToken);

            if (chat is null)
                return Result<SendMessageResult>.Failure("Chat not found.");

            try
            {
                chat.EnsureParticipant(command.SenderId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<SendMessageResult>.Failure(ex.Message);
            }

            Message message;
            try
            {
                message = Message.Create(
                    chatId: command.ChatId,
                    senderId: command.SenderId,
                    body: command.Body,
                    type: command.Type);
            }
            catch (ArgumentException ex)
            {
                return Result<SendMessageResult>.Failure(ex.Message);
            }

            await _chatRepository.AddMessageAsync(message, cancellationToken);

            // Push the new message in real-time to all connected clients in this chat group.
            var dto = new MessageDto(
                MessageId: message.Id,
                ChatId: message.ChatId,
                SenderId: message.SenderId,
                Body: message.Body,
                Type: message.Type.ToString(),
                SentAt: message.SentAt,
                ReadAt: message.ReadAt);

            await _notificationService.NotifyMessageSentAsync(
                command.ChatId, dto, cancellationToken);

            return Result<SendMessageResult>.Success(new SendMessageResult(
                MessageId: message.Id,
                ChatId: message.ChatId,
                SenderId: message.SenderId,
                Body: message.Body,
                Type: message.Type.ToString(),
                SentAt: message.SentAt));
        }
    }
}
