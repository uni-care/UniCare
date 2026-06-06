using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Aggregates.UserAggregates;


namespace UniCare.Domain.Interfaces
{
    public interface IApplicationDbContext
    {
        // Existing
        DbSet<StudentVerification> StudentVerifications { get; }

        // New DbSets for Items feature
        DbSet<Item> Items { get; }
        DbSet<UserFavorite> UserFavorites { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}