using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionHandover
{
    public interface ITransactionHandoverRepository
    {
        Task<TransactionHandover?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<TransactionHandover?> GetPendingByTransactionAndTypeAsync(Guid transactionId, HandoverType type, CancellationToken ct = default);
        Task AddAsync(TransactionHandover handover, CancellationToken ct = default);
        Task UpdateAsync(TransactionHandover handover, CancellationToken ct = default);
        Task ExpireStaleHandoversAsync(CancellationToken ct = default);
    }
}
