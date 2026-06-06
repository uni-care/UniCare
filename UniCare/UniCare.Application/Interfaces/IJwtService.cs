using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Domain.Aggregates.UserAggregates.User user);
        DateTime GetTokenExpiry();
    }

}
