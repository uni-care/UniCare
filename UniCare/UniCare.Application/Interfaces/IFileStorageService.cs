using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> UploadAsync(
            byte[] fileContent,
            string fileName,
            string folder,
            CancellationToken ct = default);

        Task DeleteAsync(string publicId, CancellationToken ct = default);
    }

    public record FileUploadResult(
        string Url,        // The secure HTTPS URL to store/show
        string PublicId    // Cloudinary public_id — store this to enable future deletions
    );
}

