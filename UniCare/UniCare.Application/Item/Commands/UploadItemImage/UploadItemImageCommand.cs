using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.Item.Commands.UploadItemImage
{
    public sealed record UploadItemImageCommand(
      Guid ItemId,
      Guid RequestingUserId,
      byte[] FileContent,
      string FileName,
      string ContentType
  ) : ICommand<Result<UploadItemImageResult>>;

    public sealed record UploadItemImageResult(string Url, string PublicId);
}
