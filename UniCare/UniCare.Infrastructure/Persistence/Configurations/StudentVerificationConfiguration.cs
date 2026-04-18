using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Infrastructure.Persistence.Configurations
{
    public class StudentVerificationConfiguration : IEntityTypeConfiguration<StudentVerification>
    {
        public void Configure(EntityTypeBuilder<StudentVerification> builder)
        {
            builder.HasKey(sv => sv.Id);

            builder.Property(sv => sv.DocumentUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sv => sv.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.Property(sv => sv.OcrExtractedName)
                .HasMaxLength(100);

            builder.Property(sv => sv.OcrExtractedUniversity)
                .HasMaxLength(150);

            builder.Property(sv => sv.OcrExtractedFaculty)
                .HasMaxLength(150);

            builder.Property(sv => sv.ReviewNotes)
                .HasMaxLength(500);

            // Keep raw OCR JSON — potentially large, store as nvarchar(max)
            builder.Property(sv => sv.OcrRawResponse)
                .HasColumnType("nvarchar(max)");

            builder.ToTable("UniCare_StudentVerifications");
        }
    }

}
