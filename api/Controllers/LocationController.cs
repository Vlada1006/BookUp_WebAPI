using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Locations;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.JsonPatch;
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
        public async Task<IActionResult> GetLocationById([FromRoute] int id)
        {
            var location = await _locationRepo.GetLocationById(id);

            if (location == null)
            {
                return NotFound("Location not found");
            }

            return Ok(location.ToLocationDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] LocationForCreateDTO locationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationModel = locationDTO.ToLocationForCreateDto();

            await _locationRepo.CreateLocation(locationModel);
            return Ok(locationModel.ToLocationDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateLocation([FromRoute] int id, [FromBody] LocationForUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationToUpdate = await _locationRepo.UpdateLocation(id, updateDTO);

            if (locationToUpdate == null)
            {
                return NotFound("Location not found");
            }

            return Ok(locationToUpdate.ToLocationDto());
        }

        [HttpPatch]
        [Route("{id:int}")]
        public async Task<IActionResult> PartialLocationUpdate([FromRoute] int id, [FromBody] JsonPatchDocument<LocationForPartialUpdateDTO> patchDoc)
        {
            if (!ModelState.IsValid || patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var patchDTO = new LocationForPartialUpdateDTO();

            patchDoc.ApplyTo(patchDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var locationToUpdate = await _locationRepo.PartialUpdateLocation(id, patchDTO);

            if (locationToUpdate == null)
            {
                return NotFound("Location not found");
            }

            return Ok(locationToUpdate.ToLocationDto());

        }

    }
}