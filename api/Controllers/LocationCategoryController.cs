using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.CategoryForLocations;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryForLocationController : ControllerBase
    {
        private readonly ILocationCategoryInterface _locationCategoryRepo;
        public CategoryForLocationController(ILocationCategoryInterface locationCategoryRepo)
        {
            _locationCategoryRepo = locationCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] QueryParameters queryParameters)
        {
            var categories = await _locationCategoryRepo.GetCategoriesForLocations(queryParameters);

            var categoriesDTO = categories.Select(u => u.ToCategoryForLocDto());

            return Ok(categoriesDTO);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var category = await _locationCategoryRepo.GetCategoryById(id);

            if (category == null)
            {
                return NotFound("There is no such category.");
            }

            return Ok(category.ToCategoryForLocDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryForLocsForCreateDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryModel = createDTO.ToCreateCategoryForLocationDto();
            await _locationCategoryRepo.CreateCategory(categoryModel);

            return Ok(categoryModel.ToCategoryForLocDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryForLocForUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryModel = await _locationCategoryRepo.UpdateCategory(id, updateDTO);

            if (categoryModel == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(categoryModel.ToCategoryForLocDto());
        }

        [HttpPatch]
        [Route("{id:int}")]
        public async Task<IActionResult> PartialUpdateCategory([FromRoute] int id, [FromBody] JsonPatchDocument<CategoryForLocForPartialUpdateDTO> patchDoc)
        {
            if (!ModelState.IsValid || patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var patchDTO = new CategoryForLocForPartialUpdateDTO();

            patchDoc.ApplyTo(patchDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryToUpdate = await _locationCategoryRepo.PartialUpdateCategory(id, patchDTO);

            if (categoryToUpdate == null)
            {
                return NotFound("Category not found.");
            }

            return Ok(categoryToUpdate.ToCategoryForLocDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryToDelete = await _locationCategoryRepo.DeleteCategory(id);

            if (categoryToDelete == null)
            {
                return NotFound("Category not found");
            }

            return Ok("Deleted!");
        }

        [HttpDelete]
        [Route("multiple")]
        public async Task<IActionResult> DeleteMultipleCategories([FromQuery] int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return NotFound("IDs not found!");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryToDelete = await _locationCategoryRepo.DeleteMultipleCategories(ids);

            if (categoryToDelete == null || !categoryToDelete.Any())
            {
                return NotFound("Categories not found!");
            }
            return Ok("Deleted!");
        }

        [HttpGet]
        [Route("{id:int}/locations")]
        public async Task<IActionResult> GetLocationsByCategoryId([FromRoute] int id)
        {
            var category = await _locationCategoryRepo.GetCategoryById(id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            var locations = await _locationCategoryRepo.GetLocationsByCategory(id);

            if (locations.Count == 0 || locations == null)
            {
                return NotFound("No locations found for the specified category");
            }
            var locationsDTO = locations.Select(u => u.ToLocationDto()).ToList();

            return Ok(locationsDTO);
        }

    }
}