using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Verification;
using UniCare.Domain.Enums;

namespace UniCare.Application.User.commands.UploadID
{
    public class UploadIdCommand : IRequest<Result<UploadIdResponseDto>>
    {
        public Guid UserId { get; set; }
        public byte[] FileContent { get; set; } = [];
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DocumentType DocumentType { get; set; }
    }
}
