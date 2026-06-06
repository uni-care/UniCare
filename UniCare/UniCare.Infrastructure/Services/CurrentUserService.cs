using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common.Interfaces;

namespace UniCare.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue("sub")
                    ?? _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdString))
                    return null;

                return Guid.TryParse(userIdString, out var userId) ? userId : null;
            }
        }

        public string? Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(JwtRegisteredClaimNames.Email);
            }
        }

        public string? Role
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.Role);
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}
