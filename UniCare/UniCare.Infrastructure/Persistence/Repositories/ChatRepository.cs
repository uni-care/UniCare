using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Infrastructure.Persistence;

namespace UniCare.Infrastructure.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly UniCareDbContext _db;

        public ChatRepository(UniCareDbContext db) => _db = db;

        public async Task<Chat?> GetByIdAsync(Guid chatId, CancellationToken ct = default)
            => await _db.Chats.FindAsync([chatId], ct);

        public async Task<Chat?> GetByTransactionIdAsync(
            Guid transactionId, CancellationToken ct = default)
            => await _db.Chats
                .FirstOrDefaultAsync(c => c.TransactionId == transactionId, ct);

        public async Task<IReadOnlyList<Chat>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default)
            => await _db.Chats
                .Where(c => c.OwnerId == userId || c.RequesterId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);

        public async Task AddAsync(Chat chat, CancellationToken ct = default)
        {
            await _db.Chats.AddAsync(chat, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Chat chat, CancellationToken ct = default)
        {
            _db.Chats.Update(chat);
            await _db.SaveChangesAsync(ct);
        }

        public async Task AddMessageAsync(Message message, CancellationToken ct = default)
        {
            await _db.Messages.AddAsync(message, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<Message>> GetMessagesAsync(
            Guid chatId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public async Task MarkMessagesReadAsync(
            Guid chatId,
            Guid readerId,
            CancellationToken ct = default)
        {
            var unread = await _db.Messages
                .Where(m => m.ChatId == chatId
                         && m.SenderId != readerId
                         && m.ReadAt == null)
                .ToListAsync(ct);

            foreach (var m in unread)
                m.MarkRead();

            await _db.SaveChangesAsync(ct);
        }
    }
}
