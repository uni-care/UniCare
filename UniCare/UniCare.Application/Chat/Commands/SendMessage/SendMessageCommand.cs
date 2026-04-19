using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Application.Chat.Commands.SendMessage
{
    public sealed record SendMessageCommand(
          Guid ChatId,
          Guid SenderId,
          string Body,
          MessageType Type = MessageType.Text
      ) : ICommand<Result<SendMessageResult>>;

    public sealed record SendMessageResult(
        Guid MessageId,
        Guid ChatId,
        Guid SenderId,
        string Body,
        string Type,
        DateTime SentAt
    );
}