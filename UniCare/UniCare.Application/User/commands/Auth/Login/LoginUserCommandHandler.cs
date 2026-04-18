using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Auth;
using UniCare.Domain.Aggregates.UserAggregates;
using Microsoft.AspNetCore.Identity;


namespace UniCare.Application.User.commands.Auth.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISignInService _signInService;
        private readonly IJwtService _jwtService;

        public LoginUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            ISignInService signInService,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInService = signInService;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            LoginUserCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result<AuthResponseDto>.Unauthorized("Invalid email or password.");

            var signInResult = await _signInService.CheckPasswordSignInAsync(user, request.Password);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return Result<AuthResponseDto>.Failure(
                        "Account is temporarily locked. Please try again later.", 423, "LOCKED_OUT");

                return Result<AuthResponseDto>.Unauthorized("Invalid email or password.");
            }

            var token = _jwtService.GenerateToken(user);
            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = _jwtService.GetTokenExpiry(),
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                VerificationStatus = user.VerificationStatus
            });
        }
    }

}
