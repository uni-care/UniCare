using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Api.Models;
using UniCare.Application.Item.Commands;
using UniCare.Application.Item.Commands.CreateItem;
using UniCare.Application.Item.Commands.ToggleFavorite;
using UniCare.Application.Item.Commands.UpdateItem;
using UniCare.Application.Item.Commands.UploadItemImage;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries;
using UniCare.Application.Item.Queries.GetAiRecommendations;
using UniCare.Application.Item.Queries.GetAllItems;
using UniCare.Application.Item.Queries.GetItemById;
using UniCare.Application.Common;

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

    [HttpGet]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<ItemDto>>>  GetAllItems(CancellationToken cancellationToken,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        var query = new GetAllItemsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(PaginatedResponse<ItemDto>.FromPaginatedList(result));
    }

    [HttpGet("{itemId:guid}")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
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
    [HttpPost("{itemId:guid}/images")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(UploadItemImageResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadImage(
    Guid itemId,
    IFormFile file,
    CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);

        var command = new UploadItemImageCommand(
            ItemId: itemId,
            RequestingUserId: GetCurrentUserId(),
            FileContent: ms.ToArray(),
            FileName: file.FileName,
            ContentType: file.ContentType);

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetItemById), new { itemId }, result.Data)
            : result.StatusCode switch
            {
                403 => Forbid(),
                404 => NotFound(new { error = result.ErrorMessage }),
                _ => BadRequest(new { error = result.ErrorMessage })
            };
    }

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
