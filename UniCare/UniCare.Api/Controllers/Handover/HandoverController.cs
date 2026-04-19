using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniCare.Application.Handover.Queries;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Api.Controllers.Handover
{
    [ApiController]
    [Route("api/transactions/{transactionId:guid}/handover")]
    public class HandoverController : ControllerBase
    {
        private readonly ISender _sender;

        public HandoverController(ISender sender) => _sender = sender;

        //[HttpPost("generate")]
        //[ProducesResponseType(typeof(GenerateHandoverResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Generate(
        //    Guid transactionId,
        //    [FromBody] GenerateHandoverRequest request,
        //    CancellationToken ct)
        //{
        //    var command = new GenerateHandoverCommand(
        //        TransactionId: transactionId,
        //        Type: request.Type,
        //        GeneratedForUserId: request.GeneratedForUserId,
        //        VerifiedByUserId: request.VerifiedByUserId
        //    );

        //    var result = await _sender.Send(command, ct);

        //    return result.IsSuccess
        //        ? Ok(result.Value)
        //        : BadRequest(new { error = result.Error });
        //}

        /// Verify a handover PIN.
        /// Accepts the PIN typed manually OR decoded from a QR scan — same endpoint, same field.
        //[HttpPost("verify")]
        //[ProducesResponseType(typeof(VerifyHandoverResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> Verify(
        //    Guid transactionId,
        //    [FromBody] VerifyHandoverRequest request,
        //    CancellationToken ct)
        //{
        //    var command = new VerifyHandoverCommand(
        //        TransactionId: transactionId,
        //        Type: request.Type,
        //        VerifyingUserId: request.VerifyingUserId,
        //        RawPin: request.Pin
        //    );

        //    var result = await _sender.Send(command, ct);

        //    return result.IsSuccess
        //        ? Ok(result.Value)
        //        : BadRequest(new { error = result.Error });
        //}

        /// Get the current status of a handover code (Pending / Verified / Expired).
        [HttpGet("status")]
        [ProducesResponseType(typeof(HandoverStatusResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus(
            Guid transactionId,
            [FromQuery] HandoverType type,
            CancellationToken ct)
        {
            var query = new GetHandoverStatusQuery(transactionId, type);
            var result = await _sender.Send(query, ct);

            return result.IsSuccess
                ? Ok(result.Data)
                : NotFound(new { error = result.ErrorMessage });
        }
    }

}