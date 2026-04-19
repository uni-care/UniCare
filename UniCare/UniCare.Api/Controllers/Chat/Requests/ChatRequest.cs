namespace UniCare.Api.Controllers.Chat.Requests
{
    public record GetOrCreateChatRequest(
         Guid TransactionId,
         Guid OwnerId,
         Guid RequesterId
     );

    public record SendMessageRequest(
        Guid SenderId,
        string Body
    );

    public record MarkReadRequest(
        Guid ReaderId
    );
}
