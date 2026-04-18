using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;

namespace UniCare.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _wwwRootPath;

        public FileStorageService(IWebHostEnvironment env)
            => _wwwRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public async Task<string> SaveFileAsync(byte[] fileContent, string fileName, string folder)
        {
            // Ensure the target directory exists
            var uploadsRoot = Path.Combine(_wwwRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsRoot);

            // Generate a unique file name to prevent collisions
            var extension = Path.GetExtension(fileName);
            var uniqueName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(uploadsRoot, uniqueName);

            await File.WriteAllBytesAsync(fullPath, fileContent);

            // Return relative URL (served as static file by ASP.NET Core)
            return $"/uploads/{folder}/{uniqueName}";
        }
    }

}
