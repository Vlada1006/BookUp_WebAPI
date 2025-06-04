using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Locations;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BookUp.UnitTests.ControllerTests
{
    public class LocationControllerTests
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

        [Fact]
        public async Task CreateLocation_ReturnsNewLocation()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            var createDTO = new LocationForCreateDTO
            {
                LocationName = "lala"
            };
            var locationModel = createDTO.ToLocationForCreateDto();

            A.CallTo(() => _locationRepo.CreateLocation(locationModel)).Returns(Task.FromResult(locationModel));

            var result = await controller.CreateLocation(createDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LocationDTO>(okResult.Value);
            Assert.Equal("lala", returnValue.LocationName);
        }

        [Fact]
        public async Task UpdateLocation_ReturnsUpdatedLocation()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            int id = 1;
            var updateDTO = new LocationForUpdateDTO
            {
                LocationName = "lala"
            };

            var updatedLocation = new Location
            {
                LocationId = 1,
                LocationName = "lala"
            };

            A.CallTo(() => _locationRepo.UpdateLocation(id, updateDTO)).Returns(Task.FromResult(updatedLocation));

            var result = await controller.UpdateLocation(id, updateDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LocationDTO>(okResult.Value);
            Assert.Equal("lala", updatedLocation.LocationName);
        }

        [Fact]
        public async Task UpdateLocation_ReturnsNotFound()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            int id = 99;
            var updateDTO = new LocationForUpdateDTO
            {
                LocationName = "nothing"
            };

            A.CallTo(() => _locationRepo.UpdateLocation(id, updateDTO)).Returns(Task.FromResult<Location>(null));

            var result = await controller.UpdateLocation(id, updateDTO);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PartialUpdateLocation_ReturnsOK()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            int id = 1;
            var patchDoc = new JsonPatchDocument<LocationForPartialUpdateDTO>();
            patchDoc.Replace(u => u.LocationName, "Updated");
            var updatedLocation = new Location
            {
                LocationId = id,
                LocationName = "Updated"
            };

            A.CallTo(() => _locationRepo.PartialUpdateLocation(id, A<LocationForPartialUpdateDTO>.Ignored)).Returns(Task.FromResult(updatedLocation));

            var result = await controller.PartialLocationUpdate(id, patchDoc);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<LocationDTO>(okResult.Value);
            Assert.Equal("Updated", returnValue.LocationName);
        }

        [Fact]
        public async Task PartialUpdateCategory_ReturnsNotFound()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            var id = 99;
            var patchDoc = new JsonPatchDocument<LocationForPartialUpdateDTO>();
            patchDoc.Replace(u => u.LocationName, "That`s bad");

            A.CallTo(() => _locationRepo.PartialUpdateLocation(id, A<LocationForPartialUpdateDTO>.Ignored))
            .Returns(Task.FromResult<Location>(null));

            var result = await controller.PartialLocationUpdate(id, patchDoc);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Location not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOk()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            var id = 1;
            var fakeLocation = new Location
            {
                LocationId = id,
                LocationName = "Lalala"
            };

            A.CallTo(() => _locationRepo.DeleteLocation(id)).Returns(Task.FromResult(fakeLocation));

            var result = await controller.DeleteLocation(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var id = 99;
            var fakeLocation = new Location
            {
                LocationId = id,
                LocationName = "Lalala"
            };

            var controller = new LocationController(_locationRepo);

            A.CallTo(() => _locationRepo.DeleteLocation(id)).Returns(Task.FromResult<Location>(null));

            var result = await controller.DeleteLocation(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteMultipleCategories_ReturnsDeletedCategories()
        {
            var ids = new int[] { 1, 2, 3 };
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);
            var fakeLocations = new List<Location>
            {
                new Location {LocationId = 1, LocationName= "Lala" },
                new Location {LocationId = 2, LocationName = "blabla"}
            };

            A.CallTo(() => _locationRepo.DeleteMultipleLocations(ids))
            .Returns(Task.FromResult<IEnumerable<Location>>(fakeLocations));

            var result = await controller.DeleteMultipleLocations(ids);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }

        [Fact]
        public async Task DeleteMultipleCategories_ReturnsNotFound()
        {
            var _locationRepo = A.Fake<ILocationInterface>();
            var controller = new LocationController(_locationRepo);

            var notFoundResult = await controller.DeleteMultipleLocations(null);
            var emptyResult = await controller.DeleteMultipleLocations(Array.Empty<int>());

            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.IsType<NotFoundObjectResult>(emptyResult);
        }
    }
}