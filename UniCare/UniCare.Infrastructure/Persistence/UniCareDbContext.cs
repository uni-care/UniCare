using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Infrastructure.Persistence
{
    public class UniCareDbContext : DbContext
    {
        public UniCareDbContext(DbContextOptions<UniCareDbContext> options) : base(options) { }

        public DbSet<TransactionHandover> TransactionHandovers => Set<TransactionHandover>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}
