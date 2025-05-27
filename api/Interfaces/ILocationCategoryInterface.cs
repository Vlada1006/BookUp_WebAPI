using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.CategoryForLocations;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ILocationCategoryInterface
    {
        public Task<List<CategoryForLocations>> GetCategoriesForLocations(QueryParameters queryParameters);
        public Task<CategoryForLocations?> GetCategoryById(int id);
        public Task<CategoryForLocations> CreateCategory(CategoryForLocations categoryForLocations);
        public Task<CategoryForLocations?> UpdateCategory(int id, CategoryForLocForUpdateDTO updateDTO);
        public Task<CategoryForLocations?> PartialUpdateCategory(int id, CategoryForLocForPartialUpdateDTO patchDTO);
        public Task<CategoryForLocations?> DeleteCategory(int id);
        public Task<IEnumerable<CategoryForLocations?>> DeleteMultipleCategories(int[] ids);
        public Task<List<Location>> GetLocationsByCategory(int id);
    }
}