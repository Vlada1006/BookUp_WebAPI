using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Places
{
    public class PlaceForPartialUpdateDTO
    {
        public string? PlaceName { get; set; } 
        public string? TypeOfPlace { get; set; } 
        public int? Capacity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
    }
}