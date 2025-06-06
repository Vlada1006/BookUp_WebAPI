using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IPlaceInterface
    {
        public Task<List<Place>> GetPlaces(PlaceQueryParameters queryParameters);
    }
}