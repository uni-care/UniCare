using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.User.DTOs.Verification
{
    public class OcrExtractedDataDto
    {
        public string? ExtractedName { get; set; }
        public string? ExtractedUniversity { get; set; }
        public string? ExtractedFaculty { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

}
