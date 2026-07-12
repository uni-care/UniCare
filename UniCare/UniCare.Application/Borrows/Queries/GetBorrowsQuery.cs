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
    public sealed record GetBorrowsQuery(
       Guid RequesterId,
       LoanStatusFilter? StatusFilter = null,
       Guid? OwnerId = null,
       DateTime? LoanedFrom = null,
       DateTime? LoanedTo = null,
       DateTime? ReturnDueFrom = null,
       DateTime? ReturnDueTo = null,
       LoanSortBy SortBy = LoanSortBy.LoanDate,
       bool SortDescending = true,
       int PageNumber = 1,
       int PageSize = 20
   ) : IQuery<Result<PagedBorrowsResult>>;

    public sealed record PagedBorrowsResult(
        IReadOnlyList<BorrowItemResult> Items,
        int TotalCount,
        int PageNumber,
        int PageSize,
        int TotalPages
    );

    public sealed record BorrowItemResult(
        Guid TransactionId,
        Guid ItemId,

        Guid OwnerId,
        string OwnerFullName,
        string? OwnerEmail,

        decimal AgreedPrice,

        DateTime BorrowedAt,
        DateTime? ReturnDueDate,
        DateTime? ReturnedAt,

        LoanStatus Status,
        string StatusLabel,

        bool IsOverdue
    );
}
