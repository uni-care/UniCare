using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Infrastructure.Persistence.Configurations
{
    public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            builder.ToTable("UserFavorites");
            builder.HasKey(uf => uf.Id);

            builder.Property(uf => uf.UserId).IsRequired();
            builder.Property(uf => uf.ItemId).IsRequired();

            builder.HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uf => uf.Item)
                .WithMany(i => i.FavoritedBy)
                .HasForeignKey(uf => uf.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(uf => new { uf.UserId, uf.ItemId }).IsUnique();
        }
    }
}