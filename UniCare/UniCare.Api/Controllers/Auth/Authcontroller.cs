using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCare.Application.Common;
using UniCare.Application.User.commands.Auth.Login;
using UniCare.Application.User.commands.Auth.Logout;
using UniCare.Application.User.commands.Auth.Register;
using UniCare.Application.User.DTOs;
using UniCare.Application.User.DTOs.Auth;

namespace UniCare.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;


        public AuthController(IMediator mediator)
            => _mediator = mediator;


        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterUserCommand
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
                RegistrationMethod = request.RegistrationMethod
            };

            var result = await _mediator.Send(command);
            return ToActionResult(result, StatusCodes.Status201Created);
        }


        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), 423)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginUserCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            return ToActionResult(result);
        }
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<LogoutResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var command = new LogoutUserCommand(userId);
            var result = await _mediator.Send(command);

            return ToActionResult(result);
        }

        [HttpPost("google")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
      

        private IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = 200)
        {
            if (result.IsSuccess)
                return StatusCode(successStatusCode, ApiResponse<T>.FromResult(result));

            return result.StatusCode switch
            {
                409 => Conflict(ApiResponse<T>.FromResult(result)),
                401 => Unauthorized(ApiResponse<T>.FromResult(result)),
                422 => UnprocessableEntity(ApiResponse<T>.FromResult(result)),
                _ => BadRequest(ApiResponse<T>.FromResult(result))
            };
        }
    }

}

