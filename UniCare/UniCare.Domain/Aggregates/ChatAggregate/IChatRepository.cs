using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.ChatAggregate
{
    public interface IChatRepository
    {
        Task<Chat?> GetByIdAsync(Guid chatId, CancellationToken ct = default);

        /// Returns the chat for a given transaction (no messages loaded).
        Task<Chat?> GetByTransactionIdAsync(Guid transactionId, CancellationToken ct = default);

        /// Returns all chats (without messages) for a user — used for the inbox list.
        Task<IReadOnlyList<Chat>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        Task AddAsync(Chat chat, CancellationToken ct = default);
        Task UpdateAsync(Chat chat, CancellationToken ct = default);

        Task AddMessageAsync(Message message, CancellationToken ct = default);

        /// Returns a paged slice of messages for a chat, ordered oldest-first.
        Task<IReadOnlyList<Message>> GetMessagesAsync(
            Guid chatId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);

        /// Marks all unread messages in a chat as read for the given reader.
        Task MarkMessagesReadAsync(Guid chatId, Guid readerId, CancellationToken ct = default);
    }
}
