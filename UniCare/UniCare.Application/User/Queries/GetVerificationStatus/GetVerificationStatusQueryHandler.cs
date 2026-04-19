using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.User.Queries.GetVerificationStatus
{
    public class GetVerificationStatusQueryHandler
    : IRequestHandler<GetVerificationStatusQuery, Result<VerificationStatusDto>>
    {
        private readonly UserManager<Domain.Aggregates.UserAggregates.User> _userManager;
        private readonly IApplicationDbContext _dbContext;

        public GetVerificationStatusQueryHandler(
            UserManager<Domain.Aggregates.UserAggregates.User> userManager,
            IApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<Result<VerificationStatusDto>> Handle(
            GetVerificationStatusQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                return Result<VerificationStatusDto>.NotFound("User not found.");

            // Fetch the latest verification submission (if any)
            var verification = await _dbContext.StudentVerifications
                .FirstOrDefaultAsync(sv => sv.UserId == request.UserId, cancellationToken);

            return Result<VerificationStatusDto>.Success(new VerificationStatusDto
            {
                Status = user.VerificationStatus,
                IsVerifiedStudentBadge = user.IsVerifiedStudent,
                VerifiedAt = user.VerificationBadgeGrantedAt,
                ReviewNotes = verification?.ReviewNotes,
                LastSubmittedAt = verification?.SubmittedAt
            });
        }
    }

}
