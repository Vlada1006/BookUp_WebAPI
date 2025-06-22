using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Locations;
using api.Models;

namespace api.Mappers
{
    public static class LocationMappers
    {
        public static LocationDTO ToLocationDto(this Location locationModel, bool ShowPlaces = false)
        {
            return new LocationDTO
            {
                LocationId = locationModel.LocationId,
                LocationName = locationModel.LocationName,
                Address = locationModel.Address,
                City = locationModel.City,
                ContactEmail = locationModel.ContactEmail,
                ContactPhone = locationModel.ContactPhone,
                Description = locationModel.Description,
                PhotoUrl = locationModel.PhotoUrl,
                CategoryId = locationModel.CategoryId,
                Places = ShowPlaces ? locationModel.Places?.Select(u=>u.ToPlaceDto()).ToList() : null
            };
        }

        public static Location ToLocationForCreateDto(this LocationForCreateDTO createDTO)
        {
            return new Location
            {
                LocationName = createDTO.LocationName,
                Address = createDTO.Address,
                City = createDTO.City,
                ContactEmail = createDTO.ContactEmail,
                ContactPhone = createDTO.ContactPhone,
                Description = createDTO.Description,
                PhotoUrl = createDTO.PhotoUrl,
                CategoryId = createDTO.CategoryId
            };
        }



    }
}