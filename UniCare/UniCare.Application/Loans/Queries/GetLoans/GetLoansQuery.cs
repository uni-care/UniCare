using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Application.Loans.Queries.GetLoans
{
   
        public sealed record GetLoansQuery(
            Guid OwnerId,
            LoanStatusFilter? StatusFilter = null,
            Guid? BorrowerId = null,
            DateTime? LoanedFrom = null,
            DateTime? LoanedTo = null,
            DateTime? ReturnDueFrom = null,
            DateTime? ReturnDueTo = null,
            LoanSortBy SortBy = LoanSortBy.LoanDate,
            bool SortDescending = true,
            int PageNumber = 1,
            int PageSize = 20
        ) : IQuery<Result<PagedLoansResult>>;

     
        public sealed record PagedLoansResult(
            IReadOnlyList<LoanItemResult> Items,
            int TotalCount,
            int PageNumber,
            int PageSize,
            int TotalPages
        );

      
        public sealed record LoanItemResult(
            Guid TransactionId,
            Guid ItemId,

            Guid BorrowerId,
            string BorrowerFullName,
            string? BorrowerEmail,

            decimal AgreedPrice,

            DateTime LoanedAt,
            DateTime? ReturnDueDate,
            DateTime? ReturnedAt,

            LoanStatus Status,
            string StatusLabel,

            bool IsOverdue
        );
    
}
