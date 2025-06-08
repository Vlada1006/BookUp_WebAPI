using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Places;
using api.Models;

namespace api.DTOs.Locations
{
    public class LocationDTO
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public int CategoryId { get; set; }

    }
}