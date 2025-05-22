using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _locationRepo.GetLocations();

            if (locations == null)
            {
                return BadRequest();
            }

            var locationsDTO = locations.Select(u => u.ToLocationDto());
            return Ok(locationsDTO.ToList());
        }


    }
}