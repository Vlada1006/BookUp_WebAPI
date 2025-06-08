using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Places;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BookUp.UnitTests.ControllerTests
{
    public class PlaceControllerTests
    {
        [Fact]
        public async Task GetAllPlaces_ReturnsOK()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var parameters = new PlaceQueryParameters();
            var fakePlaces = new List<Place>
            {
                new Place{PlaceId= 1, PlaceName = "place"},
                new Place{PlaceId = 2, PlaceName = "place2"}
            };

            A.CallTo(() => _placeRepo.GetPlaces(parameters)).Returns(Task.FromResult(fakePlaces));

            var result = await controller.GetAllPlaces(parameters);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PlaceDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetAllPlaces_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var parameters = new PlaceQueryParameters();

            A.CallTo(() => _placeRepo.GetPlaces(parameters)).Returns(Task.FromResult<List<Place>>(null));

            var result = await controller.GetAllPlaces(parameters);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Places not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPlaceById_ReturnsPlace()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 1;
            var fakePlace = new Place { PlaceId = 1, PlaceName = "place" };

            A.CallTo(() => _placeRepo.GetPlaceById(id)).Returns(Task.FromResult(fakePlace));

            var result = await controller.GetPlaceById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PlaceDTO>(okResult.Value);
            Assert.Equal(1, returnValue.PlaceId);
        }

        [Fact]
        public async Task GetPlaceById_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 99;

            A.CallTo(() => _placeRepo.GetPlaceById(id)).Returns(Task.FromResult<Place>(null));

            var result = await controller.GetPlaceById(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Place not found", notFoundResult.Value);
        }

        [Fact]
        public async Task CreatePlace_ReturnsOK()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var createDTO = new PlaceForCreateDTO { PlaceName = "place" };
            var placeModel = createDTO.ToPlaceForCreateDto();

            A.CallTo(() => _placeRepo.CreatePlace(placeModel)).Returns(Task.FromResult(placeModel));

            var result = await controller.CreatePlace(createDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PlaceDTO>(okResult.Value);
            Assert.Equal("place", returnValue.PlaceName);
        }

        [Fact]
        public async Task UpdatePlace_ReturnsOK()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 1;
            var updateDTO = new PlaceForUpdateDTO { PlaceName = "new place" };
            var updatedPlace = new Place { PlaceId = 1, PlaceName = "new place" };

            A.CallTo(() => _placeRepo.UpdatePlace(id, updateDTO)).Returns(Task.FromResult(updatedPlace));

            var result = await controller.UpdatePlace(id, updateDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PlaceDTO>(okResult.Value);
            Assert.Equal("new place", returnValue.PlaceName);
        }
        
        [Fact]
        public async Task UpdatePlace_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 99;
            var updateDTO = new PlaceForUpdateDTO { PlaceName = "no info" };

            A.CallTo(() => _placeRepo.UpdatePlace(id, updateDTO)).Returns(Task.FromResult<Place>(null));

            var result = await controller.UpdatePlace(id,updateDTO);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Place not found", notFoundResult.Value);
        }
    }
}