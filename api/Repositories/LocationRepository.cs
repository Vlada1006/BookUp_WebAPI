using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class LocationRepository : ILocationInterface
    {
        private readonly AppDbContext _db;
        public LocationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Location>> GetLocations(QueryParameters queryParameters)
        {
            var locations = _db.Locations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParameters.NameSearchTerm))
            {
                locations = locations.Where(u => u.LocationName.ToLower().Contains(queryParameters.NameSearchTerm.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.City))
            {
                locations = locations.Where(u => u.City == queryParameters.City);
            }

            switch (queryParameters.SortBy)
            {
                case "name":
                    locations = !queryParameters.IsDescending ? locations.OrderBy(u => u.LocationName) : locations.OrderByDescending(u => u.LocationName);
                    break;
                case "city":
                    locations = !queryParameters.IsDescending ? locations.OrderBy(u => u.City) : locations.OrderByDescending(u => u.City);
                    break;
            }

            locations = locations.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return await locations.ToListAsync();
        }

        public async Task<Location?> GetLocationById(int id)
        {
            var location = await _db.Locations.FirstOrDefaultAsync(u => u.LocationId == id);

            if (location == null)
            {
                return null;
            }

            return location;
        }
    }
}