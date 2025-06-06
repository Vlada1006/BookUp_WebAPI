using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int PlaceId { get; set; }
        public string? UserId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [ForeignKey("PlaceId")]
        public Place Place { get; set; }
    }
}