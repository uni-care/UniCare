using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chats.Queries.GetUserChats
{
    public sealed class GetUserChatsQueryHandler
         : IQueryHandler<GetUserChatsQuery, Result<IReadOnlyList<ChatSummaryResult>>>
    {
        private readonly IChatRepository _chatRepository;

        public GetUserChatsQueryHandler(IChatRepository chatRepository)
            => _chatRepository = chatRepository;

        public async Task<Result<IReadOnlyList<ChatSummaryResult>>> Handle(
            GetUserChatsQuery query,
            CancellationToken cancellationToken)
        {
            var chats = await _chatRepository.GetByUserIdAsync(query.UserId, cancellationToken);

            var results = new List<ChatSummaryResult>();

            foreach (var chat in chats)
            {
                // Fetch only the most recent message for the preview line.
                var recentPage = await _chatRepository.GetMessagesAsync(
                    chat.Id, pageNumber: 1, pageSize: 200, cancellationToken);

                var lastMessage = recentPage.LastOrDefault();

                var unreadCount = recentPage
                    .Count(m => m.SenderId != query.UserId && m.ReadAt is null);

                var otherUserId = chat.OwnerId == query.UserId
                    ? chat.RequesterId
                    : chat.OwnerId;

                results.Add(new ChatSummaryResult(
                    ChatId: chat.Id,
                    TransactionId: chat.TransactionId,
                    OtherUserId: otherUserId,
                    LastMessageBody: lastMessage?.Body,
                    LastMessageAt: lastMessage?.SentAt,
                    UnreadCount: unreadCount));
            }

            var sorted = results
                .OrderByDescending(r => r.LastMessageAt ?? DateTime.MinValue)
                .ToList();

            return Result<IReadOnlyList<ChatSummaryResult>>.Success(sorted);
        }
    }
}
