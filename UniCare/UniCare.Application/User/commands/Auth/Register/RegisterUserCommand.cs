using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Auth;
using UniCare.Domain.Enums;

namespace UniCare.Application.User.commands.Auth.Register
{
    public class RegisterUserCommand : IRequest<Result<AuthResponseDto>>
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? GoogleIdToken { get; set; }
        public RegistrationMethod RegistrationMethod { get; set; } = RegistrationMethod.Email;
    }

}
