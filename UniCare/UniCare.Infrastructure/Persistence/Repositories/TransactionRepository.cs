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

        public async Task<IReadOnlyList<Transaction>> GetByUserAsync(
            Guid userId, CancellationToken ct = default)
            => await _db.Transactions
                .Where(t => t.OwnerId == userId || t.RequesterId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<Transaction>> GetActiveByUserAsync(
            Guid userId, CancellationToken ct = default)
            => await _db.Transactions
                .Where(t =>
                    (t.OwnerId == userId || t.RequesterId == userId) &&
                    (t.Status == TransactionStatus.AwaitingHandover ||
                     t.Status == TransactionStatus.Active))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);

        /// <inheritdoc/>
        public async Task<(IReadOnlyList<Transaction> Items, int TotalCount)> GetLoansByOwnerAsync(
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
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            // Base query: only rental transactions where the caller is the owner/lender.
            var query = _db.Transactions
                .Where(t => t.OwnerId == ownerId && t.Type == TransactionType.Rental)
                .AsNoTracking();

            // ── Status filter ────────────────────────────────────────────────────────
            // "Overdue" is a derived concept (Active + past due date), so we translate
            // the user-facing LoanStatusFilter into the underlying TransactionStatus predicates.
            if (statusFilter.HasValue)
            {
                query = statusFilter.Value switch
                {
                    LoanStatusFilter.PendingApproval =>
                        query.Where(t => t.Status == TransactionStatus.PendingApproval),

                    LoanStatusFilter.AwaitingHandover =>
                        query.Where(t => t.Status == TransactionStatus.AwaitingHandover),

                    LoanStatusFilter.Active =>
                        // Active but NOT yet past due date (i.e. not overdue)
                        query.Where(t =>
                            t.Status == TransactionStatus.Active &&
                            (!t.RentalReturnDue.HasValue || t.RentalReturnDue.Value >= now)),

                    LoanStatusFilter.Overdue =>
                        query.Where(t =>
                            t.Status == TransactionStatus.Active &&
                            t.RentalReturnDue.HasValue &&
                            t.RentalReturnDue.Value < now),

                    LoanStatusFilter.Returned =>
                        query.Where(t => t.Status == TransactionStatus.Completed),

                    LoanStatusFilter.Cancelled =>
                        query.Where(t => t.Status == TransactionStatus.Cancelled),

                    _ => query
                };
            }

            // ── Optional filters ─────────────────────────────────────────────────────
            if (borrowerId.HasValue)
                query = query.Where(t => t.RequesterId == borrowerId.Value);

            if (loanedFrom.HasValue)
                query = query.Where(t => t.CreatedAt >= loanedFrom.Value);

            if (loanedTo.HasValue)
                query = query.Where(t => t.CreatedAt <= loanedTo.Value);

            if (returnDueFrom.HasValue)
                query = query.Where(t => t.RentalReturnDue.HasValue &&
                                         t.RentalReturnDue.Value >= returnDueFrom.Value);

            if (returnDueTo.HasValue)
                query = query.Where(t => t.RentalReturnDue.HasValue &&
                                         t.RentalReturnDue.Value <= returnDueTo.Value);

            // ── Total count (before pagination) ──────────────────────────────────────
            var totalCount = await query.CountAsync(ct);

            // ── Sorting ──────────────────────────────────────────────────────────────
            query = (sortBy, sortDescending) switch
            {
                (LoanSortBy.LoanDate, true) =>
                    query.OrderByDescending(t => t.CreatedAt),
                (LoanSortBy.LoanDate, false) =>
                    query.OrderBy(t => t.CreatedAt),

                (LoanSortBy.ReturnDueDate, true) =>
                    query.OrderByDescending(t => t.RentalReturnDue),
                (LoanSortBy.ReturnDueDate, false) =>
                    query.OrderBy(t => t.RentalReturnDue),

                (LoanSortBy.Status, true) =>
                    query.OrderByDescending(t => t.Status),
                (LoanSortBy.Status, false) =>
                    query.OrderBy(t => t.Status),

                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            // ── Pagination ───────────────────────────────────────────────────────────
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

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
