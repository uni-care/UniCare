using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Common;
using UniCare.Application.User.commands.UploadID;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Application.User.Queries.GetVerificationStatus;
using UniCare.Domain.Enums;

namespace UniCare.Api.Controllers
{
    [ApiController]
    [Route("api/v1/verify")]
    [Authorize]
    [Produces("application/json")]
    public class VerifyController : ControllerBase
    {
        private readonly IMediator _mediator;

        // Reject at the controller before the file even reaches the handler.
        // The handler validator also checks this, but failing early saves memory.
        private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public VerifyController(IMediator mediator)
            => _mediator = mediator;

        /// <summary>
        /// Upload a student ID document for AI-powered OCR verification.
        ///
        /// The document is sent to the configured OCR endpoint which extracts
        /// student data and returns a verdict:
        ///   - verified  → user receives the Verified Student badge immediately.
        ///   - rejected  → user is marked Rejected with a reason from the AI.
        ///   - pending   → OCR was inconclusive; queued for manual admin review.
        ///
        /// The response always includes the current VerificationStatus so the
        /// frontend can update the UI in a single round-trip.
        /// </summary>
        [HttpPost("upload-id")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<UploadIdResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UploadId(
            IFormFile file,
            [FromForm] DocumentType documentType = DocumentType.StudentId,
            CancellationToken cancellationToken = default)
        {
            // ── Controller-level guards ───────────────────────────────────────
            if (file is null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail(
                    "No file was provided.", "NO_FILE"));

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(ApiResponse<object>.Fail(
                    "File size must not exceed 5 MB.", "FILE_TOO_LARGE"));

            // ── Read file once into memory ────────────────────────────────────
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);

            // ── Build and dispatch command ────────────────────────────────────
            // MediatR routes this to UploadIdCommandHandler which:
            //   1. Saves the file via IFileStorageService
            //   2. Calls RealOcrService → AI OCR endpoint
            //   3. Maps the AI verdict to VerificationStatus on the user
            //   4. Persists everything to the database
            var command = new UploadIdCommand
            {
                UserId = GetCurrentUserId(),
                FileContent = ms.ToArray(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                DocumentType = documentType
            };

            var result = await _mediator.Send(command, cancellationToken);
            return ToActionResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Returns the current verification status for the authenticated user.
        ///
        /// StatusLabel values: "Not Submitted" | "Under Review" | "Verified" | "Rejected"
        /// IsVerifiedStudentBadge is true only when the AI (or admin) confirmed the document.
        /// ReviewNotes contains the AI rejection reason when Status = Rejected.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(ApiResponse<VerificationStatusDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus(CancellationToken cancellationToken = default)
        {
            var query = new GetVerificationStatusQuery { UserId = GetCurrentUserId() };
            var result = await _mediator.Send(query, cancellationToken);
            return ToActionResult(result);
        }

        // ── Private helpers ───────────────────────────────────────────────────

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
                409 => Conflict(ApiResponse<T>.FromResult(result)),
                422 => UnprocessableEntity(ApiResponse<T>.FromResult(result)),
                _ => BadRequest(ApiResponse<T>.FromResult(result))
            };
        }
    }
}