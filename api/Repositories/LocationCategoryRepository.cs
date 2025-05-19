using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

            categories = categories.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return await categories.ToListAsync();


        }
    }
}