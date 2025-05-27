using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Locations;
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

        public async Task<Location> CreateLocation(Location locationModel)
        {
            await _db.Locations.AddAsync(locationModel);
            await _db.SaveChangesAsync();
            return locationModel;
        }

        public async Task<Location> UpdateLocation(int id, LocationForUpdateDTO updateDTO)
        {
            var location = await _db.Locations.FirstOrDefaultAsync(u => u.LocationId == id);

            if (location == null)
            {
                return null;
            }

            location.LocationName = updateDTO.LocationName;
            location.Address = updateDTO.Address;
            location.City = updateDTO.City;
            location.ContactEmail = updateDTO.ContactEmail;
            location.ContactPhone = updateDTO.ContactPhone;
            location.Description = updateDTO.Description;
            location.PhotoUrl = updateDTO.PhotoUrl;

            await _db.SaveChangesAsync();
            return location;
        }

        public async Task<Location?> PartialUpdateLocation(int id, LocationForPartialUpdateDTO patchDTO)
        {
            var location = await _db.Locations.FirstOrDefaultAsync(u => u.LocationId == id);

            if (location == null)
            {
                return null;
            }

            if (patchDTO.LocationName != null) location.LocationName = patchDTO.LocationName;
            if (patchDTO.Address != null) location.Address = patchDTO.Address;
            if (patchDTO.City != null) location.City = patchDTO.City;
            if (patchDTO.ContactEmail != null) location.ContactEmail = patchDTO.ContactEmail;
            if (patchDTO.ContactPhone != null) location.ContactPhone = patchDTO.ContactPhone;
            if (patchDTO.Description != null) location.Description = patchDTO.Description;
            if (patchDTO.PhotoUrl != null) location.PhotoUrl = patchDTO.PhotoUrl;

            await _db.SaveChangesAsync();

            return location;
        }
    }
}