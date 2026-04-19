using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.Chats.Queries.GetUserChats
{
    public sealed record GetUserChatsQuery(
           Guid UserId
       ) : IQuery<Result<IReadOnlyList<ChatSummaryResult>>>;

    public sealed record ChatSummaryResult(
        Guid ChatId,
        Guid TransactionId,
        Guid OtherUserId,
        string? LastMessageBody,
        DateTime? LastMessageAt,
        int UnreadCount
    );
}
