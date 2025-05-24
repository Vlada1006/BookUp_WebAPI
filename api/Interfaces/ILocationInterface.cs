using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface ILocationInterface
    {
        public Task<List<Location>> GetLocations(QueryParameters queryParameters);
        public Task<Location?> GetLocationById(int id);
    }
}