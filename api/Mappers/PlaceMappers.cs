using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Places;
using api.Models;

namespace api.Mappers
{
    public static class PlaceMappers
    {
        public static PlaceDTO ToPlaceDto(this Place placeModel)
        {
            return new PlaceDTO
            {
                PlaceId = placeModel.PlaceId,
                PlaceName = placeModel.PlaceName,
                TypeOfPlace = placeModel.TypeOfPlace,
                Capacity = placeModel.Capacity,
                Price = placeModel.Price,
                IsAvailable = placeModel.IsAvailable,
                LocationId = placeModel.LocationId
            };
        }
    }
}