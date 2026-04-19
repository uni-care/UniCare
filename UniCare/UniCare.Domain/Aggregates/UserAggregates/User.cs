using Microsoft.AspNetCore.Identity;
using UniCare.Domain.Enums;

namespace UniCare.Domain.Aggregates.UserAggregates;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string? UniversityName { get; set; }
    public string? FacultyName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.NotSubmitted;
    public bool IsVerifiedStudent { get; set; } = false;
    public DateTime? VerificationBadgeGrantedAt { get; set; }
    public RegistrationMethod RegistrationMethod { get; set; } = RegistrationMethod.Email;
    public string? GoogleSubjectId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public StudentVerification? StudentVerification { get; set; }

}
