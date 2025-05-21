using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.DTOs.CategoryForLocations
{
    public class CategoryForLocsForCreateDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}