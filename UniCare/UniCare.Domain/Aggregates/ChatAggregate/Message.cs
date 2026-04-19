using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.ChatAggregate
{
    public class Message
    {
        public Guid Id { get; private set; }
        public Guid ChatId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Body { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }

        /// Null until the other participant reads it (read receipt).
        public DateTime? ReadAt { get; private set; }

        public MessageType Type { get; private set; }

        // EF Core navigation property (not part of domain logic).
        public Chat Chat { get; private set; } = null!;

        private Message() { } // EF Core

        public static Message Create(
            Guid chatId,
            Guid senderId,
            string body,
            MessageType type = MessageType.Text)
        {
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Message body cannot be empty.", nameof(body));

            if (body.Length > 2000)
                throw new ArgumentException("Message body cannot exceed 2000 characters.", nameof(body));

            return new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = senderId,
                Body = body.Trim(),
                SentAt = DateTime.UtcNow,
                ReadAt = null,
                Type = type
            };
        }

        /// Marks this message as read. Idempotent — only sets ReadAt once.
        public void MarkRead()
        {
            ReadAt ??= DateTime.UtcNow;
        }
    }
}
