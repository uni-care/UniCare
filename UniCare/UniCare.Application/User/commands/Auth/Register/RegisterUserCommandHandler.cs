using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Interfaces;
using UniCare.Application.User.DTOs.Auth;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Enums;
using UniCare.Application.Interfaces;

namespace UniCare.Application.User.commands.Auth.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<Domain.Aggregates.UserAggregates.User> _userManager;
        private readonly IJwtService _jwtService;

        public RegisterUserCommandHandler(
            UserManager<Domain.Aggregates.UserAggregates.User> userManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            RegisterUserCommand request,
            CancellationToken cancellationToken)
        {
            // Google registration is handled by a dedicated command/handler.
            if (request.RegistrationMethod == RegistrationMethod.Google)
                return Result<AuthResponseDto>.Failure("Use the /auth/google endpoint for Google sign-in.", 400);

            //  Duplicate Check
            if (request.RegistrationMethod == RegistrationMethod.Email)
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                    return Result<AuthResponseDto>.Failure("Email is required for Email registration.");

                var existing = await _userManager.FindByEmailAsync(request.Email);
                if (existing is not null)
                    return Result<AuthResponseDto>.Conflict("An account with this email already exists.");
            }
            else // Phone
            {
                if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                    return Result<AuthResponseDto>.Failure("Phone number is required for Phone registration.");

                var existingByPhone = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
                if (existingByPhone is not null)
                    return Result<AuthResponseDto>.Conflict("An account with this phone number already exists.");
            }

            //  Create User
            var user = new Domain.Aggregates.UserAggregates.User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email ?? request.PhoneNumber,
                PhoneNumber = request.PhoneNumber,
                RegistrationMethod = request.RegistrationMethod,
                VerificationStatus = VerificationStatus.NotSubmitted,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, request.Password!);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description);
                return Result<AuthResponseDto>.ValidationFailure(errors);
            }

            //  Generate Token
            var token = _jwtService.GenerateToken(user);
            return Result<AuthResponseDto>.Created(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = _jwtService.GetTokenExpiry(),
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                //ActiveMode = user.ActiveMode,
                VerificationStatus = user.VerificationStatus
            });
        }
    }

}
