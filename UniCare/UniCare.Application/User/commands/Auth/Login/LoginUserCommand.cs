using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Auth;
using MediatR;

namespace UniCare.Application.User.commands.Auth.Login
{
    public class LoginUserCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
