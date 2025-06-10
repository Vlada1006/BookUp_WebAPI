using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Places;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("api/places")]
    public class PlaceController : Controller
    {
        private readonly IPlaceInterface _placeRepository;

        public PlaceController(IPlaceInterface placeRepository)
        {
            _placeRepository = placeRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPlaces([FromQuery] PlaceQueryParameters queryParameters)
        {
            var places = await _placeRepository.GetPlaces(queryParameters);

            if (places == null)
            {
                return NotFound("Places not found");
            }

            var placesDTO = places.Select(u => u.ToPlaceDto());

            return Ok(placesDTO.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetPlaceById([FromRoute] int id)
        {
            var place = await _placeRepository.GetPlaceById(id);

            if (place == null)
            {
                return NotFound("Place not found");
            }

            return Ok(place.ToPlaceDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlace([FromBody] PlaceForCreateDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var placeModel = createDTO.ToPlaceForCreateDto();

            await _placeRepository.CreatePlace(placeModel);

            return Ok(placeModel.ToPlaceDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdatePlace([FromRoute] int id, [FromBody] PlaceForUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var placeToUpdate = await _placeRepository.UpdatePlace(id, updateDTO);

            if (placeToUpdate == null)
            {
                return NotFound("Place not found");
            }

            return Ok(placeToUpdate.ToPlaceDto());
        }

        [HttpPatch]
        [Route("{id:int}")]
        public async Task<IActionResult> PartialUpdatePlace([FromRoute] int id, [FromBody] JsonPatchDocument<PlaceForPartialUpdateDTO> patchDoc)
        {
            if (!ModelState.IsValid || patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var patchDTO = new PlaceForPartialUpdateDTO();

            patchDoc.ApplyTo(patchDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var placeToUpd = await _placeRepository.PartialPlaceUpdate(id, patchDTO);

            if (placeToUpd == null)
            {
                return NotFound("Place not found");
            }

            return Ok(placeToUpd.ToPlaceDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeletePlace([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var place = await _placeRepository.DeletePlace(id);

            if (place == null)
            {
                return NotFound("Place not found");
            }

            return Ok("Deleted");
        }

        [HttpDelete]
        [Route("multiple")]
        public async Task<IActionResult> DeleteMultiplePlaces(int[] ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (ids.Length == 0 || ids == null)
            {
                return NotFound("IDs not found");
            }

            var places = await _placeRepository.DeletePlaces(ids);

            if (places == null || !places.Any())
            {
                return NotFound("Places not found");
            }

            return Ok("Deleted");
        }
    
    }
}