using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.Bookings;
using api.Helpers;
using api.Interfaces;
using api.Models;
using api.Repositories;
using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TextTemplating;
using Xunit.Sdk;

namespace BookUp.UnitTests.ControllerTests
{
    public class BookingControllerTests
    {
        [Fact]
        public async Task GetBookings_ReturnsOk()
        {
            var _bookingRepo = A.Fake<IBookingInterface>();
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
            var returnValue = Assert.IsAssignableFrom<IEnumerable<BookingDTO>>(okResult.Value);
        }

        [Fact]
        public async Task GetBookings_ReturnNotFound()
        {
            var _bookingRepo = A.Fake<IBookingInterface>();
            var controller = new BookingController(_bookingRepo);
            var fakeBookings = new List<Booking>();
            var parameters = new BookingQueryParameters();

            A.CallTo(() => _bookingRepo.GetBookings(parameters)).Returns(Task.FromResult<List<Booking>>(null));

            var result = await controller.GetBookings(parameters);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Bookings not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetBookingById_ReturnsOk()
        {
            var _bookingRepo = A.Fake<IBookingInterface>();
            var controller = new BookingController(_bookingRepo);
            var id = 1;
            var fakeBooking = new Booking { BookingId = 1 };

            A.CallTo(() => _bookingRepo.GetBookingById(id)).Returns(Task.FromResult(fakeBooking));

            var result = await controller.GetBookingById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BookingDTO>(okResult.Value);
        }

        [Fact]
        public async Task GetBookingById_ReturnsNotFound()
        {
            var _bookingRepo = A.Fake<IBookingInterface>();
            var controller = new BookingController(_bookingRepo);
            var id = 99;

            A.CallTo(() => _bookingRepo.GetBookingById(id)).Returns(Task.FromResult<Booking>(null));

            var result = await controller.GetBookingById(id);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Booking not found", notFoundResult.Value);
        }
    }
}