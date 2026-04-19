using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Enums;

namespace UniCare.Application.User.DTOs.Profile
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UniversityName { get; set; }
        public string? FacultyName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public bool IsVerifiedStudent { get; set; }
        public DateTime? VerificationBadgeGrantedAt { get; set; }
        public RegistrationMethod RegistrationMethod { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
