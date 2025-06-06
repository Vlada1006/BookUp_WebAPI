using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using api.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class PlaceRepository : IPlaceInterface
    {
        private readonly AppDbContext _db;
        private readonly PlaceAvailabilityService _availabilityService;
        public PlaceRepository(AppDbContext db, PlaceAvailabilityService availabilityService)
        {
            _db = db;
            _availabilityService = availabilityService;
        }

        public async Task<List<Place>> GetPlaces(PlaceQueryParameters queryParameters)
        {
            var places = _db.Places.Include(u => u.Bookings).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                places = places.Where(u => u.PlaceName.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(queryParameters.Name))
            {
                places = places.Where(u => u.PlaceName == queryParameters.Name);
            }

            if (queryParameters.Capacity != null)
            {
                if (queryParameters.Capacity <= 10)
                {
                    places = places.Where(u => u.Capacity <= 10);
                }
                else if (queryParameters.Capacity > 10 && queryParameters.Capacity <= 50)
                {
                    places = places.Where(u => u.Capacity > 10 && u.Capacity <= 50);
                }
                else
                {
                    places = places.Where(u => u.Capacity > 50);
                }
            }

            if (queryParameters.MinPrice != null)
            {
                places = places.Where(u => u.Price >= queryParameters.MinPrice);
            }

            if (queryParameters.MaxPrice != null)
            {
                places = places.Where(u => u.Price <= queryParameters.MaxPrice);
            }

            places = _availabilityService.FilterAvailablePlaces(places, queryParameters);

            switch (queryParameters.SortBy)
            {
                case "name":
                    places = !queryParameters.IsDescending ? places.OrderBy(u => u.PlaceName) : places.OrderByDescending(u => u.PlaceName);
                    break;
                case "capacity":
                    places = !queryParameters.IsDescending ? places.OrderBy(u => u.Capacity) : places.OrderByDescending(u => u.Capacity);
                    break;
                case "price":
                    places = !queryParameters.IsDescending ? places.OrderBy(u => u.Price) : places.OrderByDescending(u => u.Price);
                    break;
            }

            places = places.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return await places.ToListAsync();
            
        }
    }
}