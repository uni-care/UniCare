using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries.GetFavorites;

namespace UniCare.Api.Controllers.Item;

/// <summary>
/// Endpoints for managing and retrieving a user's favorited marketplace items.
/// </summary>
[ApiController]
[Route("api/v1/favorites")]
[Authorize]
[Produces("application/json")]
public class FavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns the authenticated user's favorited items as a paginated list.
    /// Supports optional filtering by category and price range, and sorting by dateAdded, price, or name.
    /// </summary>
    /// <param name="pageNumber">1-based page index (default: 1).</param>
    /// <param name="pageSize">Number of items per page, between 1 and 100 (default: 10).</param>
    /// <param name="category">Optional category name filter (case-insensitive exact match).</param>
    /// <param name="minPrice">Optional minimum price filter (inclusive).</param>
    /// <param name="maxPrice">Optional maximum price filter (inclusive).</param>
    /// <param name="sortBy">Field to sort by: dateAdded | price | name (default: dateAdded).</param>
    /// <param name="sortDirection">Sort direction: asc | desc (default: desc).</param>
    /// <param name="cancellationToken">Propagates cancellation signal from the HTTP pipeline.</param>
    /// <response code="200">Paginated list of favorite items (empty list when none exist).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="422">One or more query parameters failed validation.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FavoriteItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GetFavorites(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string sortBy = "dateAdded",
        [FromQuery] string sortDirection = "desc",
        CancellationToken cancellationToken = default)
    {
        var query = new GetFavoritesQuery(
            GetCurrentUserId(),
            pageNumber,
            pageSize,
            category,
            minPrice,
            maxPrice,
            sortBy,
            sortDirection
        );

        var result = await _mediator.Send(query, cancellationToken);

        return ToActionResult(result);
    }

    private Guid GetCurrentUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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
