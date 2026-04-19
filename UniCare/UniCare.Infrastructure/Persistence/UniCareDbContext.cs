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
using UniCare.Domain.Interfaces;
using UniCare.Domain.Aggregates.ItemAggregates;
using Microsoft.AspNetCore.Identity;

namespace UniCare.Infrastructure.Persistence
{
    public class UniCareDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>,
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

            // ── TransactionHandover ───────────────────────────────────────────
            builder.Entity<TransactionHandover>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Pin)
                      .IsRequired()
                      .HasMaxLength(6);

                entity.Property(e => e.TokenHash)
                      .IsRequired()
                      .HasMaxLength(64);

                entity.Property(e => e.Type)
                      .HasConversion<int>();

                entity.Property(e => e.Status)
                      .HasConversion<int>();

                entity.Property(e => e.ExpiresAt)
                      .IsRequired();

                entity.Property(e => e.VerifiedAt)
                      .IsRequired(false);

                // Fast lookup for the verify flow
                entity.HasIndex(e => new { e.TransactionId, e.Type, e.Status });
            });

            // ── Transaction ───────────────────────────────────────────────────
            builder.Entity<Transaction>(entity =>
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

                entity.Property(e => e.CreatedAt)
                      .IsRequired();

                // Fast lookup for active-transactions-by-user query
                entity.HasIndex(e => new { e.OwnerId, e.Status });
                entity.HasIndex(e => new { e.RequesterId, e.Status });
            });

            // ── Item ──────────────────────────────────────────────────────────
            builder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Description)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.Quantity)
                      .IsRequired();

                entity.Property(e => e.IsAvailable)
                      .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                      .IsRequired();

                entity.Property(e => e.UpdatedAt)
                      .IsRequired();

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.OwnerId);
            });

            // ── Chat ──────────────────────────────────────────────────────────
            builder.Entity<Chat>(entity =>
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
            builder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Body).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Type).HasConversion<int>().IsRequired();
                entity.Property(e => e.SentAt).IsRequired();
                entity.Property(e => e.ReadAt).IsRequired(false);

                entity.HasIndex(e => new { e.ChatId, e.SenderId, e.ReadAt });
            });

            // ── StudentVerification ───────────────────────────────────────────
            builder.Entity<StudentVerification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.DocumentType)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.DocumentUrl)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.SubmittedAt)
                      .IsRequired();

                entity.Property(e => e.ReviewedAt)
                      .IsRequired(false);

                entity.Property(e => e.ReviewNotes)
                      .HasMaxLength(500);

                entity.Property(e => e.OcrExtractedName)
                      .HasMaxLength(100);

                entity.Property(e => e.OcrExtractedUniversity)
                      .HasMaxLength(150);

                entity.Property(e => e.OcrExtractedFaculty)
                      .HasMaxLength(150);

                entity.HasIndex(e => e.UserId)
                      .IsUnique();
            });

            // ── ApplicationUser ───────────────────────────────────────────────
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.ProfilePictureUrl)
                      .HasMaxLength(500);

                entity.Property(e => e.UniversityName)
                      .HasMaxLength(150);

                entity.Property(e => e.FacultyName)
                      .HasMaxLength(150);

                entity.Property(e => e.RegistrationMethod)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(e => e.VerificationStatus)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.CreatedAt)
                      .IsRequired();

                entity.Property(e => e.UpdatedAt)
                      .IsRequired();

                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.IsVerifiedStudent);
            });

            // ── Relationships ─────────────────────────────────────────────────

            // Item → ApplicationUser (Owner)
            builder.Entity<Item>()
                .HasOne(i => i.Owner)
                .WithMany()
                .HasForeignKey(i => i.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction → ApplicationUser (Owner)
            builder.Entity<Transaction>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Transaction → ApplicationUser (Requester)
            builder.Entity<Transaction>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionHandover → Transaction
            builder.Entity<TransactionHandover>()
                .HasOne<Transaction>()
                .WithMany()
                .HasForeignKey(th => th.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentVerification → ApplicationUser (one-to-one)
            builder.Entity<StudentVerification>()
                .HasOne(sv => sv.User)
                .WithOne(u => u.StudentVerification)
                .HasForeignKey<StudentVerification>(sv => sv.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Identity Table Names ──────────────────────────────────────────
            builder.Entity<ApplicationUser>().ToTable("UniCare_Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("UniCare_Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UniCare_UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UniCare_UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UniCare_UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("UniCare_RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UniCare_UserTokens");
        }
    }
}