using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.User.commands.Auth.Logout
{
    public sealed record LogoutUserCommand(Guid UserId) : ICommand<Result<LogoutResult>>;

    public sealed record LogoutResult(string Message);
}
