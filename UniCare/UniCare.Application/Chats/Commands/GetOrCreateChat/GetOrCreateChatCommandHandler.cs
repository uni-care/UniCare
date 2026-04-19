using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chats.Commands.GetOrCreateChat
{
    public sealed class GetOrCreateChatCommandHandler
          : ICommandHandler<GetOrCreateChatCommand, Result<GetOrCreateChatResult>>
    {
        private readonly IChatRepository _chatRepository;

        public GetOrCreateChatCommandHandler(IChatRepository chatRepository)
            => _chatRepository = chatRepository;

        public async Task<Result<GetOrCreateChatResult>> Handle(
            GetOrCreateChatCommand command,
            CancellationToken cancellationToken)
        {
            var existing = await _chatRepository.GetByTransactionIdAsync(
                command.TransactionId, cancellationToken);

            if (existing is not null)
                return Result<GetOrCreateChatResult>.Success(new GetOrCreateChatResult(
                    ChatId: existing.Id,
                    TransactionId: existing.TransactionId,
                    WasCreated: false));

            Chat chat;
            try
            {
                chat = Chat.Create(
                    transactionId: command.TransactionId,
                    ownerId: command.OwnerId,
                    requesterId: command.RequesterId);
            }
            catch (InvalidOperationException ex)
            {
                return Result<GetOrCreateChatResult>.Failure(ex.Message);
            }

            await _chatRepository.AddAsync(chat, cancellationToken);

            return Result<GetOrCreateChatResult>.Success(new GetOrCreateChatResult(
                ChatId: chat.Id,
                TransactionId: chat.TransactionId,
                WasCreated: true));
        }
    }
}
