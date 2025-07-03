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
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingInterface _bookingRepo;
        public BookingController(IBookingInterface bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }


        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BookingQueryParameters queryParameters)
        {
            var bookings = await _bookingRepo.GetBookings(queryParameters);
            var bookingsDTO = bookings.Select(u => u.ToBookingDto()).ToList();
            return Ok(bookingsDTO);
        }
    }
}