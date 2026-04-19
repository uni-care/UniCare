using Microsoft.EntityFrameworkCore;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Common;
using UniCare.Domain.Interfaces;
using UniCare.Domain.Aggregates.ItemAggregates;

namespace UniCare.Infrastructure.Persistence
{
    public class UniCareDbContext : IdentityDbContext<User, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>,
     IApplicationDbContext
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options) : base(options) { }

        public DbSet<TransactionHandover> TransactionHandovers => Set<TransactionHandover>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<StudentVerification> StudentVerifications => Set<StudentVerification>();
        public DbSet<Item> Items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // MUST be called first for Identity tables

            builder.ApplyConfigurationsFromAssembly(typeof(UniCareDbContext).Assembly);

                entity.Property(e => e.Pin)
                      .IsRequired()
                      .HasMaxLength(6);

                entity.Property(e => e.Type)
                      .HasConversion<int>();

                entity.Property(e => e.Status)
                      .HasConversion<int>();

                // Fast lookup for the verify flow
                entity.HasIndex(e => new { e.TransactionId, e.Type, e.Status });
            });

            // ── Transaction ───────────────────────────────────────────────────
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(e => e.Status)
                      .HasConversion<int>()
                      .IsRequired();

                entity.Property(e => e.AgreedPrice)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.RentalReturnDue)
                      .IsRequired(false);

                // Fast lookup for active-transactions-by-user query
                entity.HasIndex(e => new { e.OwnerId, e.Status });
                entity.HasIndex(e => new { e.RequesterId, e.Status });
            });
            // ── Chat ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TransactionId).IsRequired();
                entity.Property(e => e.OwnerId).IsRequired();
                entity.Property(e => e.RequesterId).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasIndex(e => e.TransactionId).IsUnique();

                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.RequesterId);

                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.Chat)
                      .HasForeignKey(m => m.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // ── Message ───────────────────────────────────────────────────────
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Body).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Type).HasConversion<int>().IsRequired();
                entity.Property(e => e.SentAt).IsRequired();
                entity.Property(e => e.ReadAt).IsRequired(false);

                entity.HasIndex(e => new { e.ChatId, e.SenderId, e.ReadAt });
            });
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
