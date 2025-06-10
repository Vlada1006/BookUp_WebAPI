using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Places;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IPlaceInterface
    {
        public Task<List<Place>> GetPlaces(PlaceQueryParameters queryParameters);
        public Task<Place?> GetPlaceById(int id);
        public Task<Place> CreatePlace(Place placeModel);
        public Task<Place?> UpdatePlace(int id, PlaceForUpdateDTO updateDTO);
        public Task<Place?> PartialPlaceUpdate(int id, PlaceForPartialUpdateDTO updateDTO);
        public Task<Place?> DeletePlace(int id);
        public Task<IEnumerable<Place?>> DeletePlaces(int[] ids);
    }
}