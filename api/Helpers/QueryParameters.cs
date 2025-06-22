using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryParameters
    {
        public string? NameSearchTerm { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        const int _maxSize = 25;
        private int _size = 50;

        public int Size
        {
            get { return _size; }
            set
            {
                _size = Math.Min(_maxSize, value);
            }
        }

        public int Page { get; set; } = 1;
        public string SortBy { get; set; } = "Id";
        public bool IsDescending { get; set; } = false;
        public bool ShowPlaces { get; set; }

    }
}