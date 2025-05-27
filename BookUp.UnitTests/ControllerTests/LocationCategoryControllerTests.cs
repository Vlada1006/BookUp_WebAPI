using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.CategoryForLocations;
using api.DTOs.Locations;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using api.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace BookUp.UnitTests.ControllerTests
{
    public class CategoryForLocationControllerTests
    {
        [Fact]
        public async Task GetAllCategories_ReturnsOK()
        {
            //arrange
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();

            var controller = new CategoryForLocationController(_categoryRepo);

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
            var controller = new CategoryForLocationController(_categoryRepo);

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
            var controller = new CategoryForLocationController(_categoryRepo);

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

            var controller = new CategoryForLocationController(_categoryRepo);
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
            var controller = new CategoryForLocationController(_categoryRepo);

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
            var controller = new CategoryForLocationController(_categoryRepo);

            var updateDTO = new CategoryForLocForUpdateDTO
            {
                CategoryName = "No"
            };

            A.CallTo(() => _categoryRepo.UpdateCategory(id, updateDTO))
            .Returns(Task.FromResult<CategoryForLocations>(null));

            var result = await controller.UpdateCategory(id, updateDTO);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PartialUpdateCategory_ReturnsOK()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);
            var id = 1;
            var patchDoc = new JsonPatchDocument<CategoryForLocForPartialUpdateDTO>();
            patchDoc.Replace(u => u.CategoryName, "Updated");

            var updatedCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Updated"
            };

            A.CallTo(() => _categoryRepo.PartialUpdateCategory(id, A<CategoryForLocForPartialUpdateDTO>.Ignored))
            .Returns(Task.FromResult(updatedCategory));

            var result = await controller.PartialUpdateCategory(id, patchDoc);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<CategoryForLocDTO>(okResult.Value);
            Assert.Equal("Updated", returnValue.CategoryName);
        }

        [Fact]
        public async Task PartialUpdateCategory_ReturnsNotFound()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);
            var id = 99;
            var patchDoc = new JsonPatchDocument<CategoryForLocForPartialUpdateDTO>();
            patchDoc.Replace(u => u.CategoryName, "That`s bad");

            A.CallTo(() => _categoryRepo.PartialUpdateCategory(id, A<CategoryForLocForPartialUpdateDTO>.Ignored))
            .Returns(Task.FromResult<CategoryForLocations>(null));

            var result = await controller.PartialUpdateCategory(id, patchDoc);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Category not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOk()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var id = 1;
            var fakeCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Lalala"
            };

            var controller = new CategoryForLocationController(_categoryRepo);

            A.CallTo(() => _categoryRepo.DeleteCategory(id)).Returns(Task.FromResult(fakeCategory));

            var result = await controller.DeleteCategory(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted!", okResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsNotFound()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var id = 99;
            var fakeCategory = new CategoryForLocations
            {
                LocationCategoryId = id,
                CategoryName = "Lalala"
            };

            var controller = new CategoryForLocationController(_categoryRepo);

            A.CallTo(() => _categoryRepo.DeleteCategory(id)).Returns(Task.FromResult<CategoryForLocations>(null));

            var result = await controller.DeleteCategory(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteMultipleCategories_ReturnsDeletedCategories()
        {
            var ids = new int[] { 1, 2, 3 };
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);
            var fakeCategories = new List<CategoryForLocations>
            {
                new CategoryForLocations {LocationCategoryId = 1, CategoryName= "Lala" },
                new CategoryForLocations {LocationCategoryId = 2, CategoryName = "blabla"}
            };

            A.CallTo(() => _categoryRepo.DeleteMultipleCategories(ids))
            .Returns(Task.FromResult<IEnumerable<CategoryForLocations>>(fakeCategories));

            var result = await controller.DeleteMultipleCategories(ids);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Deleted!", okResult.Value);
        }

        [Fact]
        public async Task DeleteMultipleCategories_ReturnsNotFound()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);

            var notFoundResult = await controller.DeleteMultipleCategories(null);
            var emptyResult = await controller.DeleteMultipleCategories(Array.Empty<int>());

            Assert.IsType<NotFoundObjectResult>(notFoundResult);
            Assert.IsType<NotFoundObjectResult>(emptyResult);
        }

        [Fact]
        public async Task GetLocationsByCategoryId_ReturnsOK()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);
            var categoryId = 1;
            var fakeCategory = new CategoryForLocations { LocationCategoryId = categoryId };
            var fakeLocations = new List<Location>
            {
                new Location { LocationId = 1, LocationName = "Coworking" },
                new Location { LocationId = 2, LocationName = "Studio" }
            };

            A.CallTo(() => _categoryRepo.GetCategoryById(categoryId)).Returns(Task.FromResult(fakeCategory));
            A.CallTo(() => _categoryRepo.GetLocationsByCategory(categoryId)).Returns(Task.FromResult(fakeLocations));

            var result = await controller.GetLocationsByCategoryId(categoryId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<LocationDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetLocationsByCategoryId_ReturnsNotFound()
        {
            var _categoryRepo = A.Fake<ILocationCategoryInterface>();
            var controller = new CategoryForLocationController(_categoryRepo);
            var categoryId = 99;
            var fakeCategory = new CategoryForLocations { LocationCategoryId = categoryId };
            var fakeLocations = new List<Location>();


            A.CallTo(() => _categoryRepo.GetCategoryById(categoryId)).Returns(Task.FromResult(fakeCategory));
            A.CallTo(() => _categoryRepo.GetLocationsByCategory(categoryId)).Returns(Task.FromResult(new List<Location>()));

            var result = await controller.GetLocationsByCategoryId(categoryId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No locations found for the specified category", notFoundResult.Value);
        }
    }
}