using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Places;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
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

            var result = await controller.UpdatePlace(id, updateDTO);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Place not found", notFoundResult.Value);
        }

        [Fact]
        public async Task PartialUpdatePlace_ReturnsOK()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 1;
            var patchDoc = new JsonPatchDocument<PlaceForPartialUpdateDTO>();
            patchDoc.Replace(u => u.PlaceName, "Updated");
            var updatedPlace = new Place
            {
                PlaceId = 1,
                PlaceName = "Updated"
            };

            A.CallTo(() => _placeRepo.PartialPlaceUpdate(id, A<PlaceForPartialUpdateDTO>.Ignored)).Returns(Task.FromResult(updatedPlace));

            var result = await controller.PartialUpdatePlace(id, patchDoc);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PlaceDTO>(okResult.Value);
            Assert.Equal("Updated", returnValue.PlaceName);
        }

        [Fact]
        public async Task PartialUpdatePlace_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            int id = 99;
            var patchDoc = new JsonPatchDocument<PlaceForPartialUpdateDTO>();
            patchDoc.Replace(u => u.PlaceName, "cannot update");

            A.CallTo(() => _placeRepo.PartialPlaceUpdate(id, A<PlaceForPartialUpdateDTO>.Ignored)).Returns(Task.FromResult<Place>(null));

            var result = await controller.PartialUpdatePlace(id, patchDoc);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Place not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeletePlace_ReturnsOk()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var id = 1;
            var fakePlace = new Place
            {
                PlaceId = id,
                PlaceName = "Lalala"
            };

            A.CallTo(() => _placeRepo.DeletePlace(id)).Returns(Task.FromResult(fakePlace));

            var result = await controller.DeletePlace(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }

        [Fact]
        public async Task DeletePlace_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var id = 99;
            var fakePlace = new Place
            {
                PlaceId = id,
                PlaceName = "Lalala"
            };

            A.CallTo(() => _placeRepo.DeletePlace(id)).Returns(Task.FromResult<Place>(null));

            var result = await controller.DeletePlace(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Place not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteMultiplePlaces_ReturnsOK()
        {
            var ids = new int[] { 1, 2, 3 };
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);
            var fakePlaces = new List<Place>
            {
                new Place {PlaceId = 1, PlaceName= "Lala" },
                new Place {PlaceId = 2, PlaceName = "blabla"}
            };

            A.CallTo(() => _placeRepo.DeletePlaces(ids))
            .Returns(Task.FromResult<IEnumerable<Place>>(fakePlaces));

            var result = await controller.DeleteMultiplePlaces(ids);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted", okResult.Value);
        }

        [Fact]
        public async Task DeleteMultipleLocations_ReturnsNotFound()
        {
            var _placeRepo = A.Fake<IPlaceInterface>();
            var controller = new PlaceController(_placeRepo);

            var notFoundResult = await controller.DeleteMultiplePlaces(null);
            var emptyResult = await controller.DeleteMultiplePlaces(Array.Empty<int>());

            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.IsType<NotFoundObjectResult>(emptyResult);
        }      
    }
}