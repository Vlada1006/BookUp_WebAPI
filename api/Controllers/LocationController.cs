using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationInterface _locationRepo;
        public LocationController(ILocationInterface locationRepo)
        {
            _locationRepo = locationRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLocations([FromQuery] QueryParameters queryParameters)
        {
            var locations = await _locationRepo.GetLocations(queryParameters);

            if (locations == null)
            {
                return BadRequest();
            }

            var locationsDTO = locations.Select(u => u.ToLocationDto());
            return Ok(locationsDTO.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var location = await _locationRepo.GetLocationById(id);

            if (location == null)
            {
                return NotFound("Location not found");
            }

            return Ok(location.ToLocationDto());
        }


    }
}