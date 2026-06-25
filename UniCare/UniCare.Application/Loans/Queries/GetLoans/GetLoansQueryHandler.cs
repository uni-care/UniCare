using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.Loans.Queries.GetLoans
{
    public sealed class GetLoansQueryHandler
           : IQueryHandler<GetLoansQuery, Result<PagedLoansResult>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager <Domain.Aggregates.UserAggregates.User> _userManager;

        public GetLoansQueryHandler(
            ITransactionRepository transactionRepository,
            UserManager<Domain.Aggregates.UserAggregates.User> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        public async Task<Result<PagedLoansResult>> Handle(
            GetLoansQuery query,
            CancellationToken cancellationToken)
        {
            // Validate that the owner user exists.
            var owner = await _userManager.FindByIdAsync(query.OwnerId.ToString());
            if (owner is null)
                return Result<PagedLoansResult>.NotFound("User not found.");

            var (transactions, totalCount) = await _transactionRepository.GetLoansByOwnerAsync(
                ownerId: query.OwnerId,
                statusFilter: query.StatusFilter,
                borrowerId: query.BorrowerId,
                loanedFrom: query.LoanedFrom,
                loanedTo: query.LoanedTo,
                returnDueFrom: query.ReturnDueFrom,
                returnDueTo: query.ReturnDueTo,
                sortBy: query.SortBy,
                sortDescending: query.SortDescending,
                pageNumber: query.PageNumber,
                pageSize: query.PageSize,
                ct: cancellationToken);

            // Batch-load unique borrower profiles to avoid N+1 queries.
            var borrowerIds = transactions
                .Select(t => t.RequesterId.ToString())
                .Distinct()
                .ToList();

            var borrowers = new Dictionary<Guid, Domain.Aggregates.UserAggregates.User>();
            foreach (var id in borrowerIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is not null)
                    borrowers[user.Id] = user;
            }

            var now = DateTime.UtcNow;

            var loanItems = transactions.Select(t =>
            {
                borrowers.TryGetValue(t.RequesterId, out var borrower);

                var status = MapToLoanStatus(t, now);

                return new LoanItemResult(
                    TransactionId: t.Id,
                    ItemId: t.ItemId,
                    BorrowerId: t.RequesterId,
                    BorrowerFullName: borrower?.FullName ?? "Unknown",
                    BorrowerEmail: borrower?.Email,
                    AgreedPrice: t.AgreedPrice,
                    LoanedAt: t.CreatedAt,
                    ReturnDueDate: t.RentalReturnDue,
                    ReturnedAt: t.Status == TransactionStatus.Completed ? t.UpdatedAt : null,
                    Status: status,
                    StatusLabel: GetStatusLabel(status),
                    IsOverdue: status == LoanStatus.Overdue
                );
            }).ToList();

            int totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            return Result<PagedLoansResult>.Success(new PagedLoansResult(
                Items: loanItems,
                TotalCount: totalCount,
                PageNumber: query.PageNumber,
                PageSize: query.PageSize,
                TotalPages: totalPages
            ));
        }

        /// <summary>
        /// Maps a Transaction's internal status + due date to the user-facing <see cref="LoanStatus"/>.
        /// Overdue is a derived state: Active + past due date.
        /// </summary>
        private static LoanStatus MapToLoanStatus(Transaction transaction, DateTime now) =>
            transaction.Status switch
            {
                TransactionStatus.PendingApproval => LoanStatus.PendingApproval,
                TransactionStatus.AwaitingHandover => LoanStatus.AwaitingHandover,
                TransactionStatus.Active when
                    transaction.RentalReturnDue.HasValue &&
                    transaction.RentalReturnDue.Value < now => LoanStatus.Overdue,
                TransactionStatus.Active => LoanStatus.Active,
                TransactionStatus.Completed => LoanStatus.Returned,
                TransactionStatus.Cancelled => LoanStatus.Cancelled,
                _ => LoanStatus.Active
            };

        private static string GetStatusLabel(LoanStatus status) => status switch
        {
            LoanStatus.PendingApproval => "Pending Approval",
            LoanStatus.AwaitingHandover => "Awaiting Handover",
            LoanStatus.Active => "Active",
            LoanStatus.Overdue => "Overdue",
            LoanStatus.Returned => "Returned",
            LoanStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
    }


}
