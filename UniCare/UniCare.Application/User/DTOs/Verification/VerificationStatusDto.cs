using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Enums;

namespace UniCare.Application.User.DTOs.Verification
{
    public class VerificationStatusDto
    {
        public VerificationStatus Status { get; set; }
        public string StatusLabel => Status switch
        {
            VerificationStatus.NotSubmitted => "Not Submitted",
            VerificationStatus.Pending => "Under Review",
            VerificationStatus.Verified => "Verified",
            VerificationStatus.Rejected => "Rejected",
            _ => "Unknown"
        };
        public bool IsVerifiedStudentBadge { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime? LastSubmittedAt { get; set; }
    }

}
