using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniCare.Application.Common;
using UniCare.Application.Common.Interfaces;
using UniCare.Application.Loans.Queries.GetLoans;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Api.Controllers.Loans
{
    
    [ApiController]
    [Route("api/v1/loans")]
    [Authorize]
    [Produces("application/json")]
    public class LoansController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public LoansController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

       
        /// | Status           | Meaning                                                  |
        /// |------------------|----------------------------------------------------------|
        /// | PendingApproval  | Transaction created; owner has not yet approved.         |
        /// | AwaitingHandover | Approved; waiting for physical item hand-off.            |
        /// | Active           | Borrower has the item; return date not yet passed.       |
        /// | Overdue          | Borrower has the item; return due date has passed.       |
        /// | Returned         | Item returned; transaction completed.                    |
        /// | Cancelled        | Transaction was cancelled before completion.             |
        ///
        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedLoansResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetLoans(
            [FromQuery] LoanStatusFilter? status = null,
            [FromQuery] Guid? borrowerId = null,
            [FromQuery] DateTime? loanedFrom = null,
            [FromQuery] DateTime? loanedTo = null,
            [FromQuery] DateTime? returnDueFrom = null,
            [FromQuery] DateTime? returnDueTo = null,
            [FromQuery] LoanSortBy sortBy = LoanSortBy.LoanDate,
            [FromQuery] bool sortDescending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (_currentUserService.UserId is not { } ownerId)
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized access.", "UNAUTHORIZED"));

            var query = new GetLoansQuery(
                OwnerId: ownerId,
                StatusFilter: status,
                BorrowerId: borrowerId,
                LoanedFrom: loanedFrom,
                LoanedTo: loanedTo,
                ReturnDueFrom: returnDueFrom,
                ReturnDueTo: returnDueTo,
                SortBy: sortBy,
                SortDescending: sortDescending,
                PageNumber: pageNumber,
                PageSize: pageSize
            );

            var result = await _mediator.Send(query, ct);
            return ToActionResult(result);
        }

        

        // ── Helpers ──────────────────────────────────────────────────────────────────

        private IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = 200)
        {
            if (result.IsSuccess)
                return StatusCode(successStatusCode, ApiResponse<T>.FromResult(result));

            return result.StatusCode switch
            {
                401 => Unauthorized(ApiResponse<T>.FromResult(result)),
                403 => StatusCode(StatusCodes.Status403Forbidden, ApiResponse<T>.FromResult(result)),
                404 => NotFound(ApiResponse<T>.FromResult(result)),
                422 => UnprocessableEntity(ApiResponse<T>.FromResult(result)),
                _ => BadRequest(ApiResponse<T>.FromResult(result))
            };
        }
    }
}
