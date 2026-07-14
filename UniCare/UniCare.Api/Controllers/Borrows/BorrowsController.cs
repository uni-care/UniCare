using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniCare.Application.Borrows.Queries;
using UniCare.Application.Common;
using UniCare.Application.Common.Interfaces;
using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Api.Controllers.Borrows
{
    [ApiController]
    [Route("api/v1/borrows")]
    [Authorize]
    [Produces("application/json")]
    public class BorrowsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public BorrowsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedBorrowsResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> GetBorrows(
            [FromQuery] LoanStatusFilter? status = null,
            [FromQuery] Guid? ownerId = null,
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
            if (_currentUserService.UserId is not { } requesterId)
                return Unauthorized(ApiResponse<object>.Fail("Unauthorized access.", "UNAUTHORIZED"));

            var query = new GetBorrowsQuery(
                RequesterId: requesterId,
                StatusFilter: status,
                OwnerId: ownerId,
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
