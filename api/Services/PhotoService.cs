using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace api.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<PhotoUploadResult> UploadPhoto(IFormFile file, string type)
        {
            var allowedTypes = new[] { "image/png", "image/jpeg" };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Unsupported file type");
            }

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"photos/{type.ToLower()}",
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return new PhotoUploadResult
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };
        }
    }
}