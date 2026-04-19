using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.ChatAggregate
{
    public interface IChatNotificationService
    {
        Task NotifyMessageSentAsync(
            Guid chatId,
            MessageDto message,
            CancellationToken ct = default);

        Task NotifyMessagesReadAsync(
            Guid chatId,
            Guid readerId,
            CancellationToken ct = default);
    }

    public record MessageDto(
        Guid MessageId,
        Guid ChatId,
        Guid SenderId,
        string Body,
        string Type,
        DateTime SentAt,
        DateTime? ReadAt
    );
}
