using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries.GetFavorites;

namespace UniCare.Api.Controllers.Item;

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
