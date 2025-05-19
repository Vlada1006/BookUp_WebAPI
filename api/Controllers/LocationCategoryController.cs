using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class LocationCategoryController : ControllerBase
    {
        private readonly ILocationCategoryInterface _locationCategoryRepo;
        public LocationCategoryController(ILocationCategoryInterface locationCategoryRepo)
        {
            _locationCategoryRepo = locationCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] QueryParameters queryParameters)
        {
            var categories = await _locationCategoryRepo.GetCategoriesForLocations(queryParameters);

            var categoriesDTO = categories.Select(u => u.ToCategoryForLocDto());

            return Ok(categories);
        }
    }
}