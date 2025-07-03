using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Places;

namespace api.DTOs.Bookings
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int PlaceId { get; set; }
        public string? UserId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}