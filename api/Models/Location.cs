using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        [MaxLength(50), MinLength(5)]
        public string LocationName { get; set; } = string.Empty;
        [MaxLength(50), MinLength(2)]
        public string Address { get; set; } = string.Empty;
        [MaxLength(50), MinLength(2)]
        public string City { get; set; } = string.Empty;
        [EmailAddress]
        public string ContactEmail { get; set; } = string.Empty;
        [Phone]
        public string ContactPhone { get; set; } = string.Empty;
        [MaxLength(250), MinLength(2)]
        public string? Description { get; set; }
        [Url]
        public string? PhotoUrl { get; set; }

        public int CategoryId { get; set; }
        public CategoryForLocations CategoryForLocations { get; set; } = null!;
        public List<Place> Places { get; set; } = new List<Place>();


    }
}