using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Bookings;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IBookingInterface
    {
        Task<List<Booking>> GetBookings(BookingQueryParameters queryParameters);
        Task<Booking?> GetBookingById(int id);

    }
}



