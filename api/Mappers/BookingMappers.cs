using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Bookings;
using api.Models;

namespace api.Mappers
{
    public static class BookingMappers
    {
        public static BookingDTO ToBookingDto(this Booking bookingModel)
        {
            return new BookingDTO
            {
                BookingId = bookingModel.BookingId,
                PlaceId = bookingModel.PlaceId,
                UserId = bookingModel.UserId,
                Date = bookingModel.Date,
                StartTime = bookingModel.StartTime,
                EndTime = bookingModel.EndTime
            };
        }
    }
}