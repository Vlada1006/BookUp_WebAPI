using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Bookings;
using api.Helpers;
using api.Models;
using api.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;

namespace BookUp.UnitTests.ControllerTests
{
    public class BookingControllerTests
    {
        [Fact]
        public async Task GetBookings_ReturnsOk()
        {
            var _bookingRepo = A.Fake<BookingRepository>();
            var controller = new BookingController(_bookingRepo);
            var parameters = new BookingQueryParameters();
            var fakeBookings = new List<Booking>
            {
                new Booking {BookingId = 1, PlaceId = 1},
                new Booking {BookingId = 2, PlaceId=2}
            };

            A.CallTo(() => _bookingRepo.GetBookings(parameters)).Returns(Task.FromResult(fakeBookings));

            var result = await controller.GetBookings(parameters);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<BookingDTO>>(okResult);
        }
    }
}