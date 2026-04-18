using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.UniversityName)
                .HasMaxLength(150);

            builder.Property(u => u.FacultyName)
                .HasMaxLength(150);

            builder.Property(u => u.ProfilePictureUrl)
                .HasMaxLength(500);

            builder.Property(u => u.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(u => u.RegistrationMethod)
                .HasConversion<string>()
                .HasMaxLength(20);

            // One-to-one relationship with StudentVerification
            builder.HasOne(u => u.StudentVerification)
                .WithOne(sv => sv.User)
                .HasForeignKey<StudentVerification>(sv => sv.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
