using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.User.DTOs.Verification;

namespace UniCare.Application.Interfaces
{
    public interface IOcrService
    {
        Task<OcrExtractedDataDto> ExtractStudentDataAsync(Stream fileStream, string fileName);
    }

}
