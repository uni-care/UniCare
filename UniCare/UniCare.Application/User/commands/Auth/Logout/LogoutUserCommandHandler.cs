using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.User.commands.Auth.Logout
{
    public sealed class LogoutUserCommandHandler
      : ICommandHandler<LogoutUserCommand, Result<LogoutResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LogoutUserCommandHandler(UserManager<ApplicationUser> userManager)
            => _userManager = userManager;

        public async Task<Result<LogoutResult>> Handle(
            LogoutUserCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());

            if (user is null)
                return Result<LogoutResult>.NotFound("User not found.");

            // Invalidate all existing tokens by refreshing the security stamp.
            // Any token issued before this point will fail validation on the next request.
            await _userManager.UpdateSecurityStampAsync(user);

            return Result<LogoutResult>.Success(new LogoutResult("Logged out successfully."));
        }
    }
}
