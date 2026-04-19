using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chats.Queries.GetConversation
{
    public sealed class GetConversationQueryHandler
         : IQueryHandler<GetConversationQuery, Result<ConversationResult>>
    {
        private readonly IChatRepository _chatRepository;

        public GetConversationQueryHandler(IChatRepository chatRepository)
            => _chatRepository = chatRepository;

        public async Task<Result<ConversationResult>> Handle(
            GetConversationQuery query,
            CancellationToken cancellationToken)
        {
            var chat = await _chatRepository.GetByIdAsync(query.ChatId, cancellationToken);

            if (chat is null)
                return Result<ConversationResult>.Failure("Chat not found.");

            try
            {
                chat.EnsureParticipant(query.RequestingUserId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Result<ConversationResult>.Failure(ex.Message);
            }

            var messages = await _chatRepository.GetMessagesAsync(
                query.ChatId, query.PageNumber, query.PageSize, cancellationToken);

            var messageResults = messages
                .Select(m => new MessageResult(
                    MessageId: m.Id,
                    SenderId: m.SenderId,
                    Body: m.Body,
                    Type: m.Type.ToString(),
                    SentAt: m.SentAt,
                    ReadAt: m.ReadAt))
                .ToList();

            return Result<ConversationResult>.Success(new ConversationResult(
                ChatId: chat.Id,
                TransactionId: chat.TransactionId,
                OwnerId: chat.OwnerId,
                RequesterId: chat.RequesterId,
                Messages: messageResults,
                PageNumber: query.PageNumber,
                PageSize: query.PageSize));
        }
    }
}
