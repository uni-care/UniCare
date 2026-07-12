using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Borrows.Queries
{
    public sealed class GetBorrowsQueryHandler
          : IQueryHandler<GetBorrowsQuery, Result<PagedBorrowsResult>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<Domain.Aggregates.UserAggregates.User> _userManager;

        public GetBorrowsQueryHandler(
            ITransactionRepository transactionRepository,
            UserManager<Domain.Aggregates.UserAggregates.User> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        public async Task<Result<PagedBorrowsResult>> Handle(
            GetBorrowsQuery query,
            CancellationToken cancellationToken)
        {
            var requester = await _userManager.FindByIdAsync(query.RequesterId.ToString());
            if (requester is null)
                return Result<PagedBorrowsResult>.NotFound("User not found.");

            var (transactions, totalCount) = await _transactionRepository.GetBorrowsByRequesterAsync(
                requesterId: query.RequesterId,
                statusFilter: query.StatusFilter,
                ownerId: query.OwnerId,
                loanedFrom: query.LoanedFrom,
                loanedTo: query.LoanedTo,
                returnDueFrom: query.ReturnDueFrom,
                returnDueTo: query.ReturnDueTo,
                sortBy: query.SortBy,
                sortDescending: query.SortDescending,
                pageNumber: query.PageNumber,
                pageSize: query.PageSize,
                ct: cancellationToken);

            var ownerIds = transactions
                .Select(t => t.OwnerId.ToString())
                .Distinct()
                .ToList();

            var owners = new Dictionary<Guid, Domain.Aggregates.UserAggregates.User>();
            foreach (var id in ownerIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is not null)
                    owners[user.Id] = user;
            }

            var now = DateTime.UtcNow;

            var borrowItems = transactions.Select(t =>
            {
                owners.TryGetValue(t.OwnerId, out var owner);

                var status = MapToLoanStatus(t, now);

                return new BorrowItemResult(
                    TransactionId: t.Id,
                    ItemId: t.ItemId,
                    OwnerId: t.OwnerId,
                    OwnerFullName: owner?.FullName ?? "Unknown",
                    OwnerEmail: owner?.Email,
                    AgreedPrice: t.AgreedPrice,
                    BorrowedAt: t.CreatedAt,
                    ReturnDueDate: t.RentalReturnDue,
                    ReturnedAt: t.Status == TransactionStatus.Completed ? t.UpdatedAt : null,
                    Status: status,
                    StatusLabel: GetStatusLabel(status),
                    IsOverdue: status == LoanStatus.Overdue
                );
            }).ToList();

            int totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

            return Result<PagedBorrowsResult>.Success(new PagedBorrowsResult(
                Items: borrowItems,
                TotalCount: totalCount,
                PageNumber: query.PageNumber,
                PageSize: query.PageSize,
                TotalPages: totalPages
            ));
        }

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
