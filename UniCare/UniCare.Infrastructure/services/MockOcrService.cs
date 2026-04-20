using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Application.User.DTOs.Verification;

namespace UniCare.Infrastructure.Services
{
    //public class MockOcrService : IOcrService
    //{
    //    private readonly ILogger<MockOcrService> _logger;

    //    public MockOcrService(ILogger<MockOcrService> logger)
    //        => _logger = logger;

    //    public Task<OcrExtractedDataDto> ExtractStudentDataAsync(Stream fileStream, string fileName)
    //    {
    //        _logger.LogWarning(
    //            "[MockOcrService] OCR stub called for file '{FileName}'. " +
    //            "Replace MockOcrService with a real CV implementation before production.", fileName);

    //        // Simulate a small async delay (as a real HTTP call would have)
    //        var result = new OcrExtractedDataDto
    //        {
    //            ExtractedName = "Mohammed Al-Nour",           // TODO: real OCR result
    //            ExtractedUniversity = "University of Khartoum",     // TODO: real OCR result
    //            ExtractedFaculty = "Faculty of Engineering",     // TODO: real OCR result
    //            ExpiryDate = new DateTime(2026, 12, 31)    // TODO: real OCR result
    //        };

    //        return Task.FromResult(result);
    //    }
    //}

}
