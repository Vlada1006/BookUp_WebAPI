using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.Locations
{
    public class LocationForPartialUpdateDTO
    {
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
