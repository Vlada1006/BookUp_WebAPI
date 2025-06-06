using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Place
    {
        public int PlaceId { get; set; }
        [MaxLength(50), MinLength(2)]
        public string PlaceName { get; set; } = string.Empty;
        [MaxLength(50), MinLength(2)]
        public string TypeOfPlace { get; set; } = string.Empty;
        public int Capacity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public List<Booking> Bookings { get; set; } = new List<Booking>();

    }
}