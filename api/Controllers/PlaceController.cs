using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
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
    }
}