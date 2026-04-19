using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Item.Commands.CreateItem;
using UniCare.Application.Item.Commands.ToggleFavorite;
using UniCare.Application.Item.Commands.UpdateItem;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries;
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

    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
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

    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [HttpPut("{itemId:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ItemDto>> UpdateItem(Guid itemId, [FromBody] UpdateItemRequest request)
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

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPost("{itemId:guid}/favorite")]
    [ProducesResponseType(typeof(FavoriteResponse), StatusCodes.Status200OK)]
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
}