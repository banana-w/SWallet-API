using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using SWallet.Repository.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWallet.Repository.Services.Implements
{
    public class CloudinaryService : ICloudinaryService
    {

        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<bool> RemoveImageAsync(string filename)
        {
            // Tạo `DeletionParams` với `publicId` (Cloudinary dùng filename làm publicId)
            var deleteParams = new DeletionParams(filename);

            // Gửi yêu cầu xóa hình ảnh đến Cloudinary
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            // Kiểm tra kết quả xóa, trả về `true` nếu thành công
            return deleteResult.Result == "ok" || deleteResult.Result == "not found";
        }



        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, string? folder = null, string? publicId = null)

        {
            try
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = folder,
                    PublicId = publicId
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        } 
    }
}
