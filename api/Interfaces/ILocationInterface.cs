using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Locations;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ILocationInterface
    {
        public Task<List<Location>> GetLocations(QueryParameters queryParameters);
        public Task<Location?> GetLocationById(int id);
        public Task<Location> CreateLocation(Location locationModel);
        public Task<Location?> UpdateLocation(int id, LocationForUpdateDTO updateDTO);
        public Task<Location?> PartialUpdateLocation(int id, LocationForPartialUpdateDTO updateDTO);
        public Task<Location?> DeleteLocation(int id);
        public Task<IEnumerable<Location?>> DeleteMultipleLocations(int[] ids);
        public Task<List<Place?>> GetPlacesByLocation(int id);
    }
}