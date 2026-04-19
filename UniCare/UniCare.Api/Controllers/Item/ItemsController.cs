using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Item.Queries.GetAllItems;
using UniCare.Application.Item.Queries;

namespace UniCare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken)
    {
        var query = new GetAllItemsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}