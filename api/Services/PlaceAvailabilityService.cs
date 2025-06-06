using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helpers;
using api.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class PlaceAvailabilityService
    {
        public IQueryable<Place> FilterAvailablePlaces(IQueryable<Place> places, PlaceQueryParameters queryParameters)
        {
            if (queryParameters.Date != null)
            {
                if (queryParameters.StartTime != null && queryParameters.EndTime != null)
                {
                    places = places.Where(u => !u.Bookings.Any(b => b.Date == queryParameters.Date &&
                                                                    b.StartTime < queryParameters.EndTime &&
                                                                    b.EndTime > queryParameters.StartTime));
                }
                else if (queryParameters.StartTime != null)
                {
                    places = places.Where(u => !u.Bookings.Any(b => b.Date == queryParameters.Date &&
                                                                    b.EndTime > queryParameters.StartTime));
                }
                else if (queryParameters.EndTime != null)
                {
                    places = places.Where(u => !u.Bookings.Any(b => b.Date == queryParameters.Date &&
                                                                    b.StartTime < queryParameters.EndTime));
                }
                else if (queryParameters.OnlyFullyAvailable == true)
                {
                    places = places.Where(u => !u.Bookings.Any(b => b.Date == queryParameters.Date));
                }
                else if (queryParameters.OnlyFullyAvailable == false)
                {
                    places = places.Include(u => u.Bookings.Where(b => b.Date == queryParameters.Date));
                }
            }

            return places;
        }
    }
}