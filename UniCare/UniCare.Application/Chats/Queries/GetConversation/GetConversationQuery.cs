using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.Chats.Queries.GetConversation
{
    public sealed record GetConversationQuery(
         Guid ChatId,
         Guid RequestingUserId,
         int PageNumber = 1,
         int PageSize = 50
     ) : IQuery<Result<ConversationResult>>;

    public sealed record ConversationResult(
        Guid ChatId,
        Guid TransactionId,
        Guid OwnerId,
        Guid RequesterId,
        IReadOnlyList<MessageResult> Messages,
        int PageNumber,
        int PageSize
    );

    public sealed record MessageResult(
        Guid MessageId,
        Guid SenderId,
        string Body,
        string Type,
        DateTime SentAt,
        DateTime? ReadAt
    );
}
