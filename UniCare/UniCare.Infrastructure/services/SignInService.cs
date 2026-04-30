using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Infrastructure.Services
{
    public class SignInService : ISignInService
    {
        private readonly SignInManager<User> _signInManager;

        public SignInService(SignInManager<User> signInManager)
            => _signInManager = signInManager;

        public async Task<SignInServiceResult> CheckPasswordSignInAsync(User user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, password, lockoutOnFailure: true);

            if (result.IsLockedOut) return SignInServiceResult.LockedOut();
            if (result.Succeeded) return SignInServiceResult.Success();
            return SignInServiceResult.Failed();
        }
    }

}
