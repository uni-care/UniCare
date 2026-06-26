using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.Commands.UploadItemImage
{
    public sealed class UploadItemImageCommandValidator
    : AbstractValidator<UploadItemImageCommand>
    {
        private static readonly string[] AllowedTypes =
            ["image/jpeg", "image/png", "image/webp"];

        private const int MaxBytes = 10 * 1024 * 1024; // 10 MB

        public UploadItemImageCommandValidator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("ItemId is required.");

            RuleFor(x => x.RequestingUserId)
                .NotEmpty().WithMessage("RequestingUserId is required.");

            RuleFor(x => x.FileContent)
                .NotEmpty().WithMessage("File content is required.")
                .Must(b => b.Length <= MaxBytes)
                .WithMessage("Image must not exceed 10 MB.");

            RuleFor(x => x.ContentType)
                .Must(ct => AllowedTypes.Contains(ct.ToLower()))
                .WithMessage("Only JPEG, PNG, and WebP images are accepted.");
        }
    }
}
