using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;
using Microsoft.EntityFrameworkCore;

namespace UniCare.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<StudentVerification> StudentVerifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
