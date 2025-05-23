using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.CategoryForLocations;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace BookUp.UnitTests.ControllerTests
{
    public class LocationCategoryControllerTests
    {
        [Fact]
        public async Task GetAllCategories_ReturnsOK()
        {
            //arrange
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();

            var controller = new LocationCategoryController(_categoryRepo);

            var parameters = new QueryParameters();

            var fakeCategories = new List<CategoryForLocations>
            {
                new CategoryForLocations { LocationCategoryId = 1, CategoryName = "Parks" },
                new CategoryForLocations { LocationCategoryId = 2, CategoryName = "Museums" }
            };

            A.CallTo(() => _categoryRepo.GetCategoriesForLocations(parameters))
            .Returns(Task.FromResult(fakeCategories));
            //act

            var result = await controller.GetAllCategories(parameters);

            //assert

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<CategoryForLocDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsOK()
        {
            int id = 1;
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new LocationCategoryController(_categoryRepo);

            var fakeCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Adventure"
            };


            A.CallTo(() => _categoryRepo.GetCategoryById(id)).Returns(Task.FromResult(fakeCategory));

            var result = await controller.GetCategoryById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryForLocDTO>(okResult.Value);
            Assert.Equal(1, returnValue.LocationCategoryId);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound()
        {
            int id = 999;
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new LocationCategoryController(_categoryRepo);

            var fakeCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Lalala"
            };

            A.CallTo(() => _categoryRepo.GetCategoryById(id)).Returns(Task.FromResult<CategoryForLocations>(null));

            var result = await controller.GetCategoryById(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreatedCategoryDTO()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var createDTO = new CategoryForLocsForCreateDTO
            {
                CategoryName = "Nature"
            };

            var controller = new LocationCategoryController(_categoryRepo);
            var categoryModel = createDTO.ToCreateCategoryForLocationDto();

            A.CallTo(() => _categoryRepo.CreateCategory(A<CategoryForLocations>.Ignored))
            .Returns(Task.FromResult(categoryModel));

            var result = await controller.CreateCategory(createDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryForLocDTO>(okResult.Value);
            Assert.Equal("Nature", returnValue.CategoryName);

        }

        [Fact]
        public async Task UpdateCategory_ReturnsUpdatedCategory()
        {
            int id = 1;
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new LocationCategoryController(_categoryRepo);

            var updateDTO = new CategoryForLocForUpdateDTO
            {
                CategoryName = "Updated"
            };

            var updatedCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Updated"
            };

            A.CallTo(() => _categoryRepo.UpdateCategory(id, updateDTO))
            .Returns(Task.FromResult(updatedCategory));

            var result = await controller.UpdateCategory(id, updateDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryForLocDTO>(okResult.Value);
            Assert.Equal("Updated", returnValue.CategoryName);
        }

        [Fact]
        public async Task UpdateCategory_ReturnsNotFound()
        {
            int id = 999;
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new LocationCategoryController(_categoryRepo);

            var updateDTO = new CategoryForLocForUpdateDTO
            {
                CategoryName = "No"
            };

            A.CallTo(() => _categoryRepo.UpdateCategory(id, updateDTO))
            .Returns(Task.FromResult<CategoryForLocations>(null));

            var result = await controller.UpdateCategory(id, updateDTO);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}