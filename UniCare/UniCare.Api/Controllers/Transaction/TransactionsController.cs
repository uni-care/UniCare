using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniCare.Api.Controllers.Transaction.Requests;
using UniCare.Application.TransactionHandover.Commands.GenerateHandover;
using UniCare.Application.Transactions.Commands.CreateTransaction;
using UniCare.Application.Transactions.Queries;

namespace UniCare.Api.Controllers.Transaction
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ISender _sender;

        public TransactionsController(ISender sender) => _sender = sender;

       
        [HttpPost]
        [ProducesResponseType(typeof(CreateTransactionResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransactionRequest request,
            CancellationToken ct)
        {
            var command = new CreateTransactionCommand(
                ItemId: request.ItemId,
                OwnerId: request.OwnerId,
                RequesterId: request.RequesterId,
                Type: request.Type,
                AgreedPrice: request.AgreedPrice,
                RentalReturnDue: request.RentalReturnDue
            );

            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetCode), new { id = result.Value!.TransactionId }, result.Value)
                : BadRequest(new { error = result.Error });
        }



        [HttpGet("{id:guid}/code")]
        [ProducesResponseType(typeof(GenerateHandoverResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCode(
            Guid id,
            [FromQuery] Guid generatedForUserId,
            [FromQuery] Guid verifiedByUserId,
            CancellationToken ct)
        {
            var stageResult = await _sender.Send(new GetCurrentHandoverStageQuery(id), ct);

            if (!stageResult.IsSuccess)
                return NotFound(new { error = stageResult.Error });

            var generateCommand = new GenerateHandoverCommand(
                TransactionId: id,
                Type: stageResult.Value!.NextHandoverType,
                GeneratedForUserId: generatedForUserId,
                VerifiedByUserId: verifiedByUserId
            );

            var generateResult = await _sender.Send(generateCommand, ct);

            return generateResult.IsSuccess
                ? Ok(generateResult.Value)
                : BadRequest(new { error = generateResult.Error });
        }


        [HttpPost("{id:guid}/verify-code")]
        [ProducesResponseType(typeof(VerifyAndAdvanceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyCode(
            Guid id,
            [FromBody] VerifyCodeRequest request,
            CancellationToken ct)
        {
            var command = new VerifyAndAdvanceTransactionCommand(
                TransactionId: id,
                VerifyingUserId: request.VerifyingUserId,
                RawPin: request.Pin
            );

            var result = await _sender.Send(command, ct);

            if (!result.IsSuccess)
                return result.Error == "Transaction not found."
                    ? NotFound(new { error = result.Error })
                    : BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<ActiveTransactionResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(
            [FromQuery] Guid userId,
            CancellationToken ct)
        {
            var result = await _sender.Send(new GetActiveTransactionsQuery(userId), ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(new { error = result.Error });
        }
    }
}
