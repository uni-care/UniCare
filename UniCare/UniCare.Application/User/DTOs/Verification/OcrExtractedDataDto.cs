using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.User.DTOs.Verification
{
    public enum OcrVerdict
    {
        Verified = 1,
        Rejected = 2
    }

    public class OcrExtractedDataDto
    {
        public string? ExtractedUniversity { get; set; }
        public string? ExtractedFaculty { get; set; }
        public bool? is_approved { get; set; }
        public string? RawApiResponse { get; set; }
    }
}
