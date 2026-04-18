using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionHandover;
using UniCare.Infrastructure.Persistence;

namespace UniCare.Infrastructure.Repositories
{
    public class TransactionHandoverRepository : ITransactionHandoverRepository
    {
        private readonly UniCareDbContext _db;

        public TransactionHandoverRepository(UniCareDbContext db) => _db = db;

        public async Task<TransactionHandover?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.TransactionHandovers.FindAsync([id], ct);

        public async Task<TransactionHandover?> GetPendingByTransactionAndTypeAsync(
            Guid transactionId, HandoverType type, CancellationToken ct = default)
            => await _db.TransactionHandovers
                .Where(h => h.TransactionId == transactionId
                         && h.Type == type
                         && h.Status == HandoverStatus.Pending)
                .FirstOrDefaultAsync(ct);

        public async Task AddAsync(TransactionHandover handover, CancellationToken ct = default)
        {
            await _db.TransactionHandovers.AddAsync(handover, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(TransactionHandover handover, CancellationToken ct = default)
        {
            _db.TransactionHandovers.Update(handover);
            await _db.SaveChangesAsync(ct);
        }

        public async Task ExpireStaleHandoversAsync(CancellationToken ct = default)
        {
            var stale = await _db.TransactionHandovers
                .Where(h => h.Status == HandoverStatus.Pending && h.ExpiresAt < DateTime.UtcNow)
                .ToListAsync(ct);

            foreach (var h in stale) h.Expire();
            await _db.SaveChangesAsync(ct);
        }
    }

}
