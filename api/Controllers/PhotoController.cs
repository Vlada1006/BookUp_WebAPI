using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/photos")]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        public PhotoController(IPhotoService photoService)
        {
            _photoService = photoService;
        }


        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] IEnumerable<IFormFile> files, [FromQuery] PhotoType type)
        {
            if (files == null || !files.Any())
            {
                return BadRequest("No files uploaded");
            }

            var results = new List<PhotoUploadResult>();

            foreach (var file in files)
            {
                try
                {
                    var result = await _photoService.UploadPhoto(file, type.ToString().ToLower());
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload file {file.FileName}:{ex.Message}");
                }
            }

            return Ok(results);
        }
    }
}