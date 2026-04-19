using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.User.commands.UploadID
{
    public class UploadIdCommandValidator : AbstractValidator<UploadIdCommand>
    {
        private static readonly string[] AllowedContentTypes =
        [
            "image/jpeg", "image/png", "image/webp", "application/pdf"
        ];

        private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public UploadIdCommandValidator()
        {
            RuleFor(x => x.FileContent)
                .NotEmpty().WithMessage("Document file is required.")
                .Must(bytes => bytes.Length <= MaxFileSizeBytes)
                    .WithMessage("File size must not exceed 5 MB.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Content type is required.")
                .Must(ct => AllowedContentTypes.Contains(ct.ToLower()))
                    .WithMessage("Only JPEG, PNG, WebP, and PDF documents are accepted.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }

}
