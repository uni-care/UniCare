using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Enums;

namespace UniCare.Domain.Aggregates.UserAggregates;

public class StudentVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string DocumentUrl { get; set; } = string.Empty;
    public string? OcrExtractedName { get; set; }
    public string? OcrExtractedUniversity { get; set; }
    public string? OcrExtractedFaculty { get; set; }
    public DateTime? OcrExpiryDate { get; set; }
    public string? OcrRawResponse { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public ApplicationUser User { get; set; } = null!;
}