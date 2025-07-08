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

            if (bookings == null)
            {
                return NotFound("Bookings not found");
            }
            var bookingsDTO = bookings.Select(u => u.ToBookingDto()).ToList();
            return Ok(bookingsDTO);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetBookingById([FromRoute] int id)
        {
            var booking = await _bookingRepo.GetBookingById(id);

            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            return Ok(booking.ToBookingDto());
        }

    }
}