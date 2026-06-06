using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.Interfaces
{
    public class SignInServiceResult
    {
        public bool Succeeded { get; init; }
        public bool IsLockedOut { get; init; }
        public bool IsNotAllowed { get; init; }

        public static SignInServiceResult Success()
            => new() { Succeeded = true };

        public static SignInServiceResult Failed()
            => new() { Succeeded = false };

        public static SignInServiceResult LockedOut()
            => new() { IsLockedOut = true };
    }

    public interface ISignInService
    {
        Task<SignInServiceResult> CheckPasswordSignInAsync(Domain.Aggregates.UserAggregates.User user, string password);
    }

}
