using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionAggregate
{
    public interface ITransactionRepository
    {
        public interface ITransactionRepository
        {
            Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
           
            Task<IReadOnlyList<Transaction>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);

            Task AddAsync(Transaction transaction, CancellationToken ct = default);
            Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
        }

    }
}
