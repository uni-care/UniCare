using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.User.DTOs.Verification
{
    public enum OcrVerdict
    {
        Pending = 0,
        Verified = 1,
        Rejected = 2
    }
    public class OcrExtractedDataDto
    {

        public string? ExtractedUniversity { get; set; }
        public string? ExtractedFaculty { get; set; }
        public OcrVerdict Verdict { get; set; } = OcrVerdict.Pending;
        public string? RawApiResponse { get; set; }
    }
}
