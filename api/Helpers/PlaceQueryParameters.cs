using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class PlaceQueryParameters
    {
        public string? SearchTerm { get; set; }
        public string? Name { get; set; }
        public int? Capacity { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool? OnlyFullyAvailable { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
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

    }
}