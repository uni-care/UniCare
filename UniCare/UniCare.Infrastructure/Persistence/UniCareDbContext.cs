using Microsoft.EntityFrameworkCore;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Infrastructure.Persistence
{
    public class UniCareDbContext : DbContext
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options) : base(options) { }

        public DbSet<TransactionHandover> TransactionHandovers => Set<TransactionHandover>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Chat> Chats => Set<Chat>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── TransactionHandover ───────────────────────────────────────────
            modelBuilder.Entity<TransactionHandover>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TokenHash)
                      .IsRequired()
                      .HasMaxLength(64);   // SHA-256 hex is always 64 chars

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
        }
    }
}
