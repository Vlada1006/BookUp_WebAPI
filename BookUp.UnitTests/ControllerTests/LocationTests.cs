using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Locations;
using api.Helpers;
using api.Interfaces;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace BookUp.UnitTests.ControllerTests
{
    public class LocationTests
    {
        [Fact]
        public async Task GetAllLocations_ReturnsOk()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            var parameters = new QueryParameters();
            var fakeLocations = new List<Location>
            {
                new Location {LocationId = 1, LocationName = "Coworking"},
                new Location {LocationId = 2, LocationName = "Studio"}
            };

            A.CallTo(() => _locationRepo.GetLocations(parameters)).Returns(Task.FromResult(fakeLocations));

            var result = await controller.GetAllLocations(parameters);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LocationDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetLocationById_ReturnsOk()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            int id = 1;
            var fakeLocation = new Location { LocationId = 1, LocationName = "Location" };

            A.CallTo(() => _locationRepo.GetLocationById(id)).Returns(Task.FromResult(fakeLocation));

            var result = await controller.GetLocationById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LocationDTO>(okResult.Value);
            Assert.Equal(1, returnValue.LocationId);
        }

        [Fact]
        public async Task GetLocationById_ReturnsNotFound()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            int id = 99;

            A.CallTo(() => _locationRepo.GetLocationById(id)).Returns(Task.FromResult<Location>(null));

            var result = await controller.GetLocationById(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

        }
    }
}