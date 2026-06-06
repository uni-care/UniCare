using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniCare.Api.Controllers.Transaction.Requests;
using UniCare.Application.Common.Interfaces;
using UniCare.Application.Handover.Commands.GenerateHandover;
using UniCare.Application.Transactions.Commands.CreateTransaction;
using UniCare.Application.Transactions.Queries;
using UniCare.Application.Transactions.Queries.GetActiveTransactions;
using UniCare.Application.Transactions.Queries.GetAllTransactions;
using UniCare.Application.Transactions.Queries.GetCurrentHandoverStage;

namespace UniCare.Api.Controllers.Transaction
{
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        public TransactionsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

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

            var result = await _mediator.Send(command, ct);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetCode), new { id = result.Data!.TransactionId }, result.Data)
                : BadRequest(new { error = result.ErrorMessage });
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
            var stageResult = await _mediator.Send(new GetCurrentHandoverStageQuery(id), ct);

            if (!stageResult.IsSuccess)
                return NotFound(new { error = stageResult.ErrorMessage });

            var generateCommand = new GenerateHandoverCommand(
                TransactionId: id,
                Type: stageResult.Data!.NextHandoverType,
                GeneratedForUserId: generatedForUserId,
                VerifiedByUserId: verifiedByUserId
            );

            var generateResult = await _mediator.Send(generateCommand, ct);

            return generateResult.IsSuccess
                ? Ok(generateResult.Data)
                : BadRequest(new { error = generateResult.ErrorMessage });
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

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return result.ErrorMessage == "Transaction not found."
                    ? NotFound(new { error = result.ErrorMessage })
                    : BadRequest(new { error = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("active")]
        [ProducesResponseType(typeof(IReadOnlyList<ActiveTransactionResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(
            [FromQuery] Guid userId,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new GetActiveTransactionsQuery(userId), ct);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { error = result.ErrorMessage });
        }
        [HttpGet("all")]
        [ProducesResponseType(typeof(IReadOnlyList<AllTransactionsResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserId(
           CancellationToken ct)
        {
            Guid userId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var result = await _mediator.Send(new GetAllTransactionsQuery(userId), ct);

            return result.IsSuccess
                ? Ok(result.Data)
                : BadRequest(new { error = result.ErrorMessage });
        }
    }
}
