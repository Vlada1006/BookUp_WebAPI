using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
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

        public async Task<List<Location>> GetLocations()
        {
            var locations = await _db.Locations.ToListAsync();
            return locations;
        }
    }
}