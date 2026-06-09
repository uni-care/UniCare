using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Infrastructure.Settings;

namespace UniCare.Infrastructure.Services
{
    public class CloudinaryFileStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorageService(IOptions<CloudinarySettings> options)
        {
            var s = options.Value;
            var account = new Account(s.CloudName, s.ApiKey, s.ApiSecret);
            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<FileUploadResult> UploadAsync(
     byte[] fileContent,
     string fileName,
     string folder,
     CancellationToken ct = default)
        {
            using var stream = new MemoryStream(fileContent);

            //  Decide resource type by extension 
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var resourceType = extension switch
            {
                ".mp4" or ".mov" or ".avi" => ResourceType.Video,
                _ => ResourceType.Image
            };

            var fileDescription = new FileDescription(fileName, stream);

            UploadResult result;

            switch (resourceType)
            {
                case ResourceType.Video:
                    result = await _cloudinary.UploadAsync(new VideoUploadParams
                    {
                        File = fileDescription,
                        Folder = folder,
                        UseFilename = false,
                        UniqueFilename = true,
                        Overwrite = false
                    }, ct);
                    break;

                default: 
                    result = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = fileDescription,
                        Folder = folder,
                        UseFilename = false,
                        UniqueFilename = true,
                        Overwrite = false
                    }, ct);
                    break;
            }

            if (result.Error is not null)
                throw new InvalidOperationException(
                    $"Cloudinary upload failed: {result.Error.Message}");

            return new FileUploadResult(
                Url: result.SecureUrl.ToString(),
                PublicId: result.PublicId);
        }

        public async Task DeleteAsync(string publicId, CancellationToken ct = default)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error is not null)
                throw new InvalidOperationException(
                    $"Cloudinary deletion failed: {result.Error.Message}");
        }
    }

}
