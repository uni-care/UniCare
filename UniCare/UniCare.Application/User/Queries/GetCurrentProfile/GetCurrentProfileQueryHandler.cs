using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Profile;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.User.Queries.GetCurrentProfile
{
    public class GetCurrentProfileQueryHandler
    : IRequestHandler<GetCurrentProfileQuery, Result<UserProfileDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCurrentProfileQueryHandler(UserManager<ApplicationUser> userManager)
            => _userManager = userManager;

        public async Task<Result<UserProfileDto>> Handle(
            GetCurrentProfileQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                return Result<UserProfileDto>.NotFound("User not found.");

            return Result<UserProfileDto>.Success(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UniversityName = user.UniversityName,
                FacultyName = user.FacultyName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                VerificationStatus = user.VerificationStatus,
                IsVerifiedStudent = user.IsVerifiedStudent,
                VerificationBadgeGrantedAt = user.VerificationBadgeGrantedAt,
                RegistrationMethod = user.RegistrationMethod,
                CreatedAt = user.CreatedAt
            });
        }
    }

}
