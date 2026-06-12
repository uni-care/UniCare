using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Api.Models;
using UniCare.Application.Item.Commands;
using UniCare.Application.Item.Commands.CreateItem;
using UniCare.Application.Item.Commands.ToggleFavorite;
using UniCare.Application.Item.Commands.UpdateItem;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries;
using UniCare.Application.Item.Queries.GetAiRecommendations;
using UniCare.Application.Item.Queries.GetAllItems;
using UniCare.Application.Item.Queries.GetItemById;

namespace UniCare.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    /// <summary>
    /// Returns all marketplace items.
    /// </summary>
    /// <response code="200">List of items.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken)
    {
        var query = new GetAllItemsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single item by its ID.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <response code="200">The requested item.</response>
    /// <response code="401">Not authenticated.</response>
    /// <response code="404">Item not found.</response>
    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> GetItemById(Guid itemId)
    {
        try
        {
            var currentUserId = User.Identity?.IsAuthenticated == true
                ? GetCurrentUserId()
                : (Guid?)null;

            var query = new GetItemByIdQuery(itemId, currentUserId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Creates a new marketplace item owned by the authenticated user.
    /// </summary>
    /// <param name="request">Item creation payload.</param>
    /// <response code="201">Item created successfully.</response>
    /// <response code="400">Validation error or bad request.</response>
    /// <response code="401">Not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemRequest request)
    {
        var currentUserId = GetCurrentUserId();

        var command = new CreateItemCommand(
            request.Title,
            request.Description,
            request.Price,
            request.Currency,
            request.CategoryId,
            request.AvailableFrom,
            request.AvailableTo,
            request.Location,
            request.ImageUrls,
            currentUserId
        );

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetItemById), new { itemId = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing marketplace item. Only the item owner may perform this operation.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to update.</param>
    /// <param name="request">Fields to update. All fields are optional; omitted fields remain unchanged.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Item updated successfully; returns the full updated item.</response>
    /// <response code="400">Request body is malformed.</response>
    /// <response code="401">Not authenticated.</response>
    /// <response code="403">Authenticated user is not the owner of this item.</response>
    /// <response code="404">Item or referenced category not found.</response>
    /// <response code="422">One or more validation errors in the request body.</response>
    [HttpPut("{itemId:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<ItemDto>> UpdateItem(
        Guid itemId,
        [FromBody] UpdateItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            var command = new UpdateItemCommand(
                itemId,
                currentUserId,
                request.Title,
                request.Description,
                request.Price,
                request.Currency,
                request.CategoryId,
                request.Status,
                request.AvailableFrom,
                request.AvailableTo,
                request.Location,
                request.ImageUrls
            );

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggles the favorite status of an item for the authenticated user.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <response code="200">Favorite toggled; response indicates new favorite state.</response>
    /// <response code="401">Not authenticated.</response>
    /// <response code="404">Item not found.</response>
    [HttpPost("{itemId:guid}/favorite")]
    [ProducesResponseType(typeof(FavoriteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FavoriteResponse>> ToggleFavorite(Guid itemId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var command = new ToggleFavoriteCommand(itemId, currentUserId);
            var isFavorited = await _mediator.Send(command);

            return Ok(new FavoriteResponse(isFavorited, isFavorited ? "Added to favorites" : "Removed from favorites"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RecommendationsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<ItemDto>>> Get([FromQuery] string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt)) return BadRequest();

            var result = await _mediator.Send(new GetAiRecommendationsQuery(prompt));
            return Ok(result);
        }
    }
}