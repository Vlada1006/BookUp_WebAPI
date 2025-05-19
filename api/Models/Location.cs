using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Location
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
        public CategoryForLocations CategoryForLocations { get; set; } = null!;
        public List<Place> Places { get; set; } = new List<Place>();


    }
}