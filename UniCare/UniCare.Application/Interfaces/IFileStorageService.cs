using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(byte[] fileContent, string fileName, string folder);
    }
}
