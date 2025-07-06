using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    public class BookingRepository : IBookingInterface
    {
        private readonly AppDbContext _db;
        public BookingRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Booking>> GetBookings(BookingQueryParameters queryParameters)
        {
            var bookings = _db.Bookings.AsQueryable();

            if (queryParameters.SortBy == "userId")
            {
                bookings = !queryParameters.IsDescending ? bookings.OrderBy(u => u.UserId) : bookings.OrderByDescending(u => u.UserId);
            }

            bookings = bookings.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            return await bookings.ToListAsync();
        }

        public async Task<Booking> GetBookingById(int id)
        {
            var booking = await _db.Bookings.FirstOrDefaultAsync(u => u.BookingId == id);

            if (booking == null)
            {
                return null;
            }

            return booking;
        }
    }

}