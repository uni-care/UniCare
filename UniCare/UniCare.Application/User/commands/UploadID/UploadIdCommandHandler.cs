using MediatR;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.Interfaces;
using UniCare.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace UniCare.Application.User.commands.UploadID
{
    public class UploadIdCommandHandler : IRequestHandler<UploadIdCommand, Result<UploadIdResponseDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOcrService _ocrService;
        private readonly IFileStorageService _fileStorage;

        public UploadIdCommandHandler(
            IApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IOcrService ocrService,
            IFileStorageService fileStorage)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _ocrService = ocrService;
            _fileStorage = fileStorage;
        }

        public async Task<Result<UploadIdResponseDto>> Handle(
            UploadIdCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                return Result<UploadIdResponseDto>.NotFound("User not found.");
            var folder = $"verifications/{request.UserId}";
            var documentUrl = await _fileStorage.SaveFileAsync(request.FileContent, request.FileName, folder);
            using var stream = new MemoryStream(request.FileContent);
            var ocrData = await _ocrService.ExtractStudentDataAsync(stream, request.FileName);
            var verification = await _dbContext.StudentVerifications
                .FirstOrDefaultAsync(sv => sv.UserId == request.UserId, cancellationToken);

            if (verification is null)
            {
                verification = new StudentVerification { UserId = request.UserId };
                _dbContext.StudentVerifications.Add(verification);
            }

            verification.DocumentType = request.DocumentType;
            verification.DocumentUrl = documentUrl;
            verification.OcrExtractedName = ocrData.ExtractedName;
            verification.OcrExtractedUniversity = ocrData.ExtractedUniversity;
            verification.OcrExtractedFaculty = ocrData.ExtractedFaculty;
            verification.OcrExpiryDate = ocrData.ExpiryDate;
            verification.SubmittedAt = DateTime.UtcNow;
            verification.ReviewedAt = null;
            verification.ReviewNotes = null;
            user.VerificationStatus = VerificationStatus.Pending;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<UploadIdResponseDto>.Created(new UploadIdResponseDto
            {
                VerificationId = verification.Id,
                Status = VerificationStatus.Pending,
                SubmittedAt = verification.SubmittedAt,
                ExtractedData = ocrData
            });
        }
    }
}
