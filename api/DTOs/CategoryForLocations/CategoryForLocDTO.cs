using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.DTOs.CategoryForLocations
{
    public class CategoryForLocDTO
    {
        public int LocationCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Location>? Locations { get; set; }
    }
}