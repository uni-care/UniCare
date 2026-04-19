using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Common;
using UniCare.Application.User.commands.UploadID;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Application.User.Queries.GetVerificationStatus;
using UniCare.Domain.Enums;

namespace UniCare.Api.Controllers.Verification
{

    [ApiController]
    [Route("api/v1/verify")]
    [Authorize]
    [Produces("application/json")]
    public class VerifyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VerifyController(IMediator mediator)
            => _mediator = mediator;


        [HttpPost("upload-id")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<UploadIdResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UploadId(
            IFormFile file,
            [FromForm] DocumentType documentType = DocumentType.StudentId)
        {
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("No file was provided.", "NO_FILE"));

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var command = new UploadIdCommand
            {
                UserId = GetCurrentUserId(),
                FileContent = ms.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                DocumentType = documentType
            };

            var result = await _mediator.Send(command);
            return ToActionResult(result, StatusCodes.Status201Created);
        }


        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<VerificationStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStatus()
        {
            var query = new GetVerificationStatusQuery { UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query);
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
                404 => NotFound(ApiResponse<T>.FromResult(result)),
                422 => UnprocessableEntity(ApiResponse<T>.FromResult(result)),
                _ => BadRequest(ApiResponse<T>.FromResult(result))
            };
        }
    }

}
