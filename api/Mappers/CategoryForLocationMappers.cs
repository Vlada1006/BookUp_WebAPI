using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.CategoryForLocations;
using api.Models;

namespace api.Mappers
{
    public static class CategoryForLocationMappers
    {
        public static CategoryForLocDTO ToCategoryForLocDto(this CategoryForLocations categoryModel)
        {
            return new CategoryForLocDTO
            {
                LocationCategoryId = categoryModel.LocationCategoryId,
                CategoryName = categoryModel.CategoryName,
                Description = categoryModel.Description
            };
        }

        public static CategoryForLocations ToCreateCategoryForLocationDto(this CategoryForLocsForCreateDTO createDTO)
        {
            return new CategoryForLocations
            {
                CategoryName = createDTO.CategoryName,
                Description = createDTO.Description
            };
        }
    }
}