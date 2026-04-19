using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Infrastructure.Persistence;

namespace UniCare.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly UniCareDbContext _db;

        public TransactionRepository(UniCareDbContext db) => _db = db;

        public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Transactions.FindAsync([id], ct);

        public async Task<IReadOnlyList<Transaction>> GetActiveByUserAsync(
            Guid userId, CancellationToken ct = default)
            => await _db.Transactions
                .Where(t =>
                    (t.OwnerId == userId || t.RequesterId == userId) &&
                    (t.Status == TransactionStatus.AwaitingHandover ||
                     t.Status == TransactionStatus.Active))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

        public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
        {
            await _db.Transactions.AddAsync(transaction, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
        {
            _db.Transactions.Update(transaction);
            await _db.SaveChangesAsync(ct);
        }
    }
}
