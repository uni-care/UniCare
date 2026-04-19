using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Enums;

namespace UniCare.Application.User.DTOs.Verification
{
    public class UploadIdResponseDto
    {
        public Guid VerificationId { get; set; }
        public VerificationStatus Status { get; set; }
        public OcrExtractedDataDto ExtractedData { get; set; } = new();
        public DateTime SubmittedAt { get; set; }
    }

}
