using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionAggregate
{
    public interface ITransactionRepository
    {
        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Transaction>> GetByUserAsync(Guid userId, CancellationToken ct = default);
        Task<IReadOnlyList<Transaction>> GetActiveByUserAsync(Guid userId, CancellationToken ct = default);

       
        Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetLoansByOwnerAsync(
            Guid ownerId,
            LoanStatusFilter? statusFilter,
            Guid? borrowerId,
            DateTime? loanedFrom,
            DateTime? loanedTo,
            DateTime? returnDueFrom,
            DateTime? returnDueTo,
            LoanSortBy sortBy,
            bool sortDescending,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default);

        Task AddAsync(Transaction transaction, CancellationToken ct = default);
        Task UpdateAsync(Transaction transaction, CancellationToken ct = default);
    }

   
    public enum LoanStatusFilter
    {
        PendingApproval = 1,
        AwaitingHandover = 2,
        Active = 3,
        Overdue = 4,
        Returned = 5,
        Cancelled = 6
    }

    public enum LoanSortBy
    {
        LoanDate = 1,
        ReturnDueDate = 2,
        Status = 3
    }

}
