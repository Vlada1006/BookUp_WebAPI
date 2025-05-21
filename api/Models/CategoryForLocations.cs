using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class CategoryForLocations
    {
        [Key]
        public int LocationCategoryId { get; set; }
        [MaxLength(50), MinLength(2)]
        public string CategoryName { get; set; } = string.Empty;
        [MaxLength(250), MinLength(5)]
        public string? Description { get; set; }
        public List<Location> Locations { get; set; } = new List<Location>();
    }
}