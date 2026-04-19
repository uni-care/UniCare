using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.ChatAggregate
{
    public class Chat
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Guid OwnerId { get; private set; }
        public Guid RequesterId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Message> _messages = new();
        public IReadOnlyList<Message> Messages => _messages.AsReadOnly();

        private Chat() { } 

        public static Chat Create(Guid transactionId, Guid ownerId, Guid requesterId)
        {
            if (ownerId == requesterId)
                throw new InvalidOperationException("Owner and requester cannot be the same user.");

            return new Chat
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                OwnerId = ownerId,
                RequesterId = requesterId,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// Validates that the given user is a participant in this chat.
        public void EnsureParticipant(Guid userId)
        {
            if (userId != OwnerId && userId != RequesterId)
                throw new UnauthorizedAccessException(
                    "User is not a participant in this chat.");
        }
    }
}
