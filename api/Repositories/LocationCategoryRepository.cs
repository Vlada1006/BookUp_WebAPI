using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.CategoryForLocations;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class LocationCategoryRepository : ILocationCategoryInterface
    {
        private readonly AppDbContext _db;

        public LocationCategoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryForLocations>> GetCategoriesForLocations(QueryParameters queryParameters)
        {
            var categories = _db.LocCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParameters.NameSearchTerm))
            {
                categories = categories.Where(u => u.CategoryName.ToLower().Contains(queryParameters.NameSearchTerm.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                categories = categories.Where(u => u.CategoryName == queryParameters.Name);
            }

           if (queryParameters.SortBy == "name")
            {
                categories = !queryParameters.IsDescending ? categories.OrderBy(u => u.CategoryName) : categories.OrderByDescending(u => u.CategoryName);
            }

            categories = categories.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return await categories.ToListAsync();
        }

        public async Task<CategoryForLocations?> GetCategoryById(int id)
        {
            var category = await _db.LocCategories.FirstOrDefaultAsync(u => u.LocationCategoryId == id);

            if (category == null)
            {
                return null;
            }

            return category;
        }

        public async Task<CategoryForLocations> CreateCategory(CategoryForLocations categoryForLocations)
        {
            await _db.LocCategories.AddAsync(categoryForLocations);
            await _db.SaveChangesAsync();

            return categoryForLocations;
        }

        public async Task<CategoryForLocations?> UpdateCategory(int id, CategoryForLocForUpdateDTO updateDTO)
        {
            var existingCategory = await _db.LocCategories.FirstOrDefaultAsync(u => u.LocationCategoryId == id);

            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.CategoryName = updateDTO.CategoryName;
            existingCategory.Description = updateDTO.Description;

            await _db.SaveChangesAsync();

            return existingCategory;
        }

        public async Task<CategoryForLocations?> PartialUpdateCategory(int id, CategoryForLocForPartialUpdateDTO patchDTO)
        {
            var existingCategory = await _db.LocCategories.FirstOrDefaultAsync(u => u.LocationCategoryId == id);

            if (existingCategory == null)
            {
                return null;
            }

            if (patchDTO.CategoryName != null) existingCategory.CategoryName = patchDTO.CategoryName;
            if (patchDTO.Description != null) existingCategory.Description = patchDTO.Description;

            await _db.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<CategoryForLocations?> DeleteCategory(int id)
        {
            var category = await _db.LocCategories.FirstOrDefaultAsync(u => u.LocationCategoryId == id);

            if (category == null)
            {
                return null;
            }

            _db.LocCategories.Remove(category);
            await _db.SaveChangesAsync();

            return category;
        }

        public async Task<IEnumerable<CategoryForLocations?>> DeleteMultipleCategories(int[] ids)
        {
            var categories = new List<CategoryForLocations>();

            foreach (var id in ids)
            {
                var category = await _db.LocCategories.FirstOrDefaultAsync(u => u.LocationCategoryId == id);

                if (category == null)
                {
                    continue;
                }

                categories.Add(category);
            }

            if (categories.Count == 0)
            {
                return Enumerable.Empty<CategoryForLocations>();
            }

            _db.LocCategories.RemoveRange(categories);
            await _db.SaveChangesAsync();

            return categories;
        }

        public async Task<List<Location>> GetLocationsByCategory(int id)
        {
            var locations = await _db.Locations.Where(u => u.CategoryId == id).ToListAsync();
            return locations;
        }
    }
}