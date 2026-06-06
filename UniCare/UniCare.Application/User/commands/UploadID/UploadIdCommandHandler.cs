using MediatR;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Interfaces;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Enums;
using UniCare.Domain.Interfaces;

namespace UniCare.Application.User.commands.UploadID
{
    public class UploadIdCommandHandler : IRequestHandler<UploadIdCommand, Result<UploadIdResponseDto>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<Domain.Aggregates.UserAggregates.User> _userManager;
        private readonly IOcrService _ocrService;
        private readonly IFileStorageService _fileStorage;

        public UploadIdCommandHandler(
            IApplicationDbContext dbContext,
            UserManager<Domain.Aggregates.UserAggregates.User> userManager,
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
            var documentUrl = await _fileStorage.SaveFileAsync(
                request.FileContent, request.FileName, folder);

            using var stream = new MemoryStream(request.FileContent);

            var ocrData = await _ocrService.ExtractStudentDataAsync(
                fileStream: stream,
                fileName: request.FileName,
                userId: request.UserId.ToString(),
                docType: MapDocType(request.DocumentType));

      
            var (newStatus, isVerified, badgeGrantedAt) =
                ResolveVerificationOutcome(ocrData.Verdict);

          
            var verification = await _dbContext.StudentVerifications
                .FirstOrDefaultAsync(sv => sv.UserId == request.UserId, cancellationToken);

            if (verification is null)
            {
                verification = new StudentVerification { UserId = request.UserId };
                _dbContext.StudentVerifications.Add(verification);
            }

            verification.DocumentType = request.DocumentType;
            verification.DocumentUrl = documentUrl;

            verification.OcrExtractedName = null;   
            verification.OcrExtractedUniversity = ocrData.ExtractedUniversity;
            verification.OcrExtractedFaculty = ocrData.ExtractedFaculty;
            verification.OcrExpiryDate = null;  

            verification.OcrRawResponse = ocrData.RawApiResponse;

            verification.SubmittedAt = DateTime.UtcNow;
            verification.ReviewedAt = newStatus is VerificationStatus.Verified
                                                  or VerificationStatus.Rejected
                                    ? DateTime.UtcNow
                                    : null;
            verification.ReviewNotes = null;

            user.VerificationStatus = newStatus;
            user.IsVerifiedStudent = isVerified;
            user.VerificationBadgeGrantedAt = badgeGrantedAt;

            if (newStatus == VerificationStatus.Verified)
            {
                user.UniversityName = ocrData.ExtractedUniversity ?? user.UniversityName;
                user.FacultyName = ocrData.ExtractedFaculty ?? user.FacultyName;
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<UploadIdResponseDto>.Created(new UploadIdResponseDto
            {
                VerificationId = verification.Id,
                Status = newStatus,
                SubmittedAt = verification.SubmittedAt,
                ExtractedData = ocrData
            });
        }

        private static (
            VerificationStatus status,
            bool isVerifiedStudent,
            DateTime? badgeGrantedAt)
        ResolveVerificationOutcome(OcrVerdict verdict) => verdict switch
        {
            OcrVerdict.Verified => (
                VerificationStatus.Verified, true, DateTime.UtcNow),

            OcrVerdict.Rejected => (
                VerificationStatus.Rejected, false, null),

            _ => (VerificationStatus.Pending, false, null)
        };


        private static string MapDocType(UniCare.Domain.Enums.DocumentType docType) =>
            docType switch
            {
                UniCare.Domain.Enums.DocumentType.StudentId => "student_id",
                UniCare.Domain.Enums.DocumentType.NominationCard => "nomination_card",
                UniCare.Domain.Enums.DocumentType.NationalId => "national_id",
                UniCare.Domain.Enums.DocumentType.GraduationCertificate => "graduation_certificate",
                _ => "student_id"
            };
    }
}
