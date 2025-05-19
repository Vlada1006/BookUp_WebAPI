using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class LocationsCategory
    {
        [Key]
        public int LocationCategoryId { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
    }
}