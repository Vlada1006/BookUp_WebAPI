using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.DTOs.Places;
using api.Helpers;
using api.Interfaces;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
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

            if (queryParameters.MaxCapacity != null)
            {
                if (queryParameters.MaxCapacity <= 10)
                {
                    places = places.Where(u => u.Capacity <= 10);
                }
                else if (queryParameters.MaxCapacity > 10 && queryParameters.MaxCapacity <= 50)
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

        public async Task<Place?> GetPlaceById(int id)
        {
            var place = await _db.Places.FirstOrDefaultAsync(u => u.PlaceId == id);

            if (place == null)
            {
                return null;
            }

            return place;
        }

        public async Task<Place> CreatePlace(Place placeModel)
        {
            await _db.Places.AddAsync(placeModel);
            await _db.SaveChangesAsync();

            return placeModel;

        }

        public async Task<Place?> UpdatePlace(int id, PlaceForUpdateDTO updateDTO)
        {
            var place = await _db.Places.FirstOrDefaultAsync(u => u.PlaceId == id);

            if (place == null)
            {
                return null;
            }

            place.PlaceName = updateDTO.PlaceName;
            place.TypeOfPlace = updateDTO.TypeOfPlace;
            place.Capacity = updateDTO.Capacity;
            place.Price = place.Price;

            await _db.SaveChangesAsync();

            return place;
        }

        public async Task<Place?> PartialPlaceUpdate(int id, PlaceForPartialUpdateDTO patchDTO)
        {
            var place = await _db.Places.FirstOrDefaultAsync(u => u.PlaceId == id);

            if (place == null)
            {
                return null;
            }

            if (patchDTO.PlaceName != null) place.PlaceName = patchDTO.PlaceName;
            if (patchDTO.TypeOfPlace != null) place.TypeOfPlace = patchDTO.TypeOfPlace;
            if (patchDTO.Capacity.HasValue) place.Capacity = patchDTO.Capacity.Value;
            if (patchDTO.Price.HasValue) place.Price = patchDTO.Price.Value;

            await _db.SaveChangesAsync();
            return place;

        }

        public async Task<Place?> DeletePlace(int id)
        {
            var place = await _db.Places.FirstOrDefaultAsync(u => u.PlaceId == id);

            if (place == null)
            {
                return null;
            }

            _db.Places.Remove(place);
            await _db.SaveChangesAsync();

            return place;
        }

        public async Task<IEnumerable<Place?>> DeletePlaces(int[] ids)
        {
            var places = new List<Place>();

            foreach (var id in ids)
            {
                var place = await _db.Places.FirstOrDefaultAsync(u => u.PlaceId == id);

                if (place == null)
                {
                    continue;
                }

                places.Add(place);
            }

            if (places.Count == 0 || !places.Any())
            {
                return Enumerable.Empty<Place>();
            }

            _db.Places.RemoveRange(places);
            await _db.SaveChangesAsync();

            return places;
        }
    }
}