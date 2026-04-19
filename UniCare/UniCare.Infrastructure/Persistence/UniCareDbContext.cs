using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Common;
using UniCare.Domain.Interfaces;
using UniCare.Domain.Aggregates.ItemAggregates;

namespace UniCare.Infrastructure.Persistence
{
    public class UniCareDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>,
     IApplicationDbContext
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options) : base(options) { }

        public DbSet<StudentVerification> StudentVerifications => Set<StudentVerification>();
        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // MUST be called first for Identity tables

            builder.ApplyConfigurationsFromAssembly(typeof(UniCareDbContext).Assembly);

            builder.Entity<ApplicationUser>().ToTable("UniCare_Users");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>().ToTable("UniCare_Roles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>>().ToTable("UniCare_UserRoles");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>>().ToTable("UniCare_UserClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>>().ToTable("UniCare_UserLogins");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<Guid>>().ToTable("UniCare_RoleClaims");
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>().ToTable("UniCare_UserTokens");
        }
    }

}
