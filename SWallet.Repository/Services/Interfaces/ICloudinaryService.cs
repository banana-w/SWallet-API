using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Interfaces
{
    public interface ICloudinaryService
    {

        Task<bool> RemoveImageAsync(string publicId);

        Task<ImageUploadResult> UploadImageAsync(IFormFile file, string? folder = null, string? publicId = null);

    }
}
