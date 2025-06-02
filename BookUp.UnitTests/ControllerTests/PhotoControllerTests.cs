using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api.Controllers;
using api.Helpers;
using api.Interfaces;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;

namespace BookUp.UnitTests.ControllerTests
{
    public class PhotoControllerTests
    {
        [Fact]
        public async Task UploadPhoto_ReturnsPhotoUrl()
        {
            var _photoService = A.Fake<IPhotoService>();
            var controller = new PhotoController(_photoService);
            var fileContent = "This is a fake image file";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            IFormFile fakeFile = new FormFile(stream, 0, stream.Length, "files", "fakeImage.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var files = new List<IFormFile> { fakeFile };
            PhotoType photoType = PhotoType.Location;

            var fakeResult = new PhotoUploadResult { Url = "http://fakeurl.com/fakeImage.jpg" };

            A.CallTo(() => _photoService.UploadPhoto(fakeFile, photoType.ToString().ToLower())).Returns(Task.FromResult(fakeResult));

            var result = await controller.UploadPhoto(files, photoType);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<PhotoUploadResult>>(okResult.Value);
            Assert.Single(returnValue);
            Assert.Equal("http://fakeurl.com/fakeImage.jpg", returnValue[0].Url);
        }

        [Fact]
        public async Task UploadPhoto_ReturnMultiplePhotosUrl()
        {
            var _photoService = A.Fake<IPhotoService>();
            var controller = new PhotoController(_photoService);
            var files = new List<IFormFile>();
            var expectedResults = new List<PhotoUploadResult>();

            for (int i = 1; i <= 2; i++)
            {
                var stream = new MemoryStream(Encoding.UTF8.GetBytes($"Fake content {i}"));
                var file = new FormFile(stream, 0, stream.Length, "files", $"file{i}.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/gpeg"
                };
                files.Add(file);

                var fileResult = new PhotoUploadResult { Url = $"http://fake.com/file{i}.jpg" };
                expectedResults.Add(fileResult);

                A.CallTo(() => _photoService.UploadPhoto(file, "location")).Returns(Task.FromResult(fileResult));
            }

            var type = PhotoType.Location;

            var result = await controller.UploadPhoto(files, type);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<List<PhotoUploadResult>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
            Assert.Equal("http://fake.com/file1.jpg", returnValue[0].Url);
            Assert.Equal("http://fake.com/file2.jpg", returnValue[1].Url);
        }

        [Fact]
        public async Task UploadPhoto_ReturnsBadRequest_WithNoFileUploaded()
        {
            var _photoService = A.Fake<IPhotoService>();
            var controller = new PhotoController(_photoService);
            var emptyFiles = new List<IFormFile>();
            var type = PhotoType.Event;

            var result = await controller.UploadPhoto(emptyFiles, type);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No files uploaded", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadPhoto_ReturnsBadRequest_WhileUploading()
        {
            var _photoService = A.Fake<IPhotoService>();
            var controller = new PhotoController(_photoService);
            var fileContent = "fake";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
            var fakeFile = new FormFile(stream, 0, stream.Length, "files", "fail.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            var files = new List<IFormFile> { fakeFile };
            var type = PhotoType.Place;

            A.CallTo(() => _photoService.UploadPhoto(fakeFile, type.ToString().ToLower()))
            .Throws(new Exception("Something went wrong"));

            var result = await controller.UploadPhoto(files, type);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("fail.jpg", badRequestResult.Value.ToString());
        }
    
    }
}