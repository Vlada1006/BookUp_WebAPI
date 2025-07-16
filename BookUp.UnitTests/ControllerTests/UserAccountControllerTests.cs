using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs.UserAccount;
using api.Interfaces;
using api.Models;
using api.Models.DTOs.UserAccount;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Xunit;

namespace BookUp.UnitTests.ControllerTests
{
    public class UserAccountControllerTests
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly UserAccountController _controller;

        public UserAccountControllerTests()
        {
            var store = A.Fake<IUserStore<User>>();
            _userManager = A.Fake<UserManager<User>>(opt =>
            opt.WithArgumentsForConstructor(() =>
            new UserManager<User>(store, null, null, null, null, null, null, null, null)));

            _emailService = A.Fake<IEmailService>();

            _controller = new UserAccountController(_userManager, _emailService);
        }

        private void SetUserContext(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }


        [Fact]
        public async Task GetUserAccountData_ReturnsOk_WhenUserExists()
        {
            var userId = "987265erdfghbn";
            var user = new User { Id = userId, UserName = "user", Email = "test@test.com", EmailConfirmed = true };

            A.CallTo(() => _userManager.Users).Returns(new List<User> { user }.AsQueryable().BuildMock());

            var result = await _controller.GetUserAccountData(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetUserAccountData_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userId = "non-exist";

            A.CallTo(() => _userManager.Users).Returns(new List<User>().AsQueryable().BuildMock());

            var result = await _controller.GetUserAccountData(userId);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSuccessful()
        {
            var userId = "dughycbifwuaschf87";
            var user = new User { Id = userId };
            SetUserContext(userId);
            var dto = new ChangePasswordDTO { CurrentPassword = "old", NewPassword = "new" };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword)).Returns(IdentityResult.Success);

            var result = await _controller.ChangePassword(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully", okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenSamePassword()
        {
            var userId = "dughycbifwu3443522chf87";
            var user = new User { Id = userId };
            SetUserContext(userId);
            var dto = new ChangePasswordDTO { CurrentPassword = "same", NewPassword = "same" };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword)).Returns(IdentityResult.Success);

            var result = await _controller.ChangePassword(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("New password can be the same as old one", badRequestResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsBadRequest_WhenFails()
        {
            var userId = "dughybvocvvhcuisgchf87";
            var user = new User { Id = userId };
            SetUserContext(userId);
            var dto = new ChangePasswordDTO { CurrentPassword = "old", NewPassword = "new" };

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword))
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid" }));

            var result = await _controller.ChangePassword(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenUserNotFound()
        {
            var userId = "dughybvocvvhcuisgchf87";
            SetUserContext(userId);

            A.CallTo(() => _userManager.FindByIdAsync(userId)).Returns((User)null);

            var result = await _controller.ChangePassword(new ChangePasswordDTO());

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User not found.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorized_WhenNoUserId()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.ChangePassword(new ChangePasswordDTO());

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("User ID not found in token.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenSuccessful()
        {
            var user = new User { Email = "test@test.com" };
            var dto = new ResetPasswordDTO { Email = user.Email, Token = "token", NewPassword = "new" };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.Email)).Returns(user);
            A.CallTo(() => _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword)).Returns(IdentityResult.Success);

            var result = await _controller.ResetPassword(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ReturnsBadRequest_WhenFails()
        {
            var user = new User { Email = "test@test.com" };
            var dto = new ResetPasswordDTO { Email = user.Email, Token = "token", NewPassword = "new" };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.Email)).Returns(user);
            A.CallTo(() => _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword))
            .Returns(IdentityResult.Failed(new IdentityError()));

            var result = await _controller.ResetPassword(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ResetPassword_ReturnsNotFound_WhenUserNotFound()
        {
            A.CallTo(() => _userManager.FindByEmailAsync("test@test.com")).Returns((User)null);

            var result = await _controller.ResetPassword(new ResetPasswordDTO { Email = "test@test.com" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOk_WithToken_WhenUserExists()
        {
            var user = new User { Email = "test@test.com" };

            A.CallTo(() => _userManager.FindByEmailAsync(user.Email)).Returns(user);
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user)).Returns("fake-token");

            var result = await _controller.ForgotPassword(new ForgotPasswordDTO { Email = user.Email });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Token", okResult.Value.ToString());
        }

        [Fact]
        public async Task ForgotPassword_ReturnsOk_WhenUserNotFound()
        {
            A.CallTo(() => _userManager.FindByEmailAsync("no@email.com")).Returns((User)null);

            var result = await _controller.ForgotPassword(new ForgotPasswordDTO { Email = "no@email.com" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("If your email is registered, a reset link will be sent.", okResult.Value);
        }

        [Fact]
        public async Task ChangeEmail_ReturnsOk_WhenSuccessful()
        {
            var user = new User { Id = "8723e6gfyugduy", Email = "old@email.com" };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id)).Returns(user);
            A.CallTo(() => _userManager.ChangeEmailAsync(user, "new@mail.com", "token")).Returns(IdentityResult.Success);
            A.CallTo(() => _userManager.GenerateEmailConfirmationTokenAsync(user)).Returns("email-token");

            var result = await _controller.ChangeEmail(user.Id, "new@mail.com", "token");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email changed successfully. Please confirm your new email.", okResult.Value);
        }

        [Fact]
        public async Task ChangeEmail_ReturnsNotFound_WhenUserNotFound()
        {
            A.CallTo(() => _userManager.FindByIdAsync("no")).Returns((User)null);

            var result = await _controller.ChangeEmail("no", "new@mail.com", "token");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task ChangeEmail_ReturnsBadRequest_WhenFails()
        {
            var user = new User { Id = "98765rdfghjd76yuejd6" };

            A.CallTo(() => _userManager.FindByIdAsync(user.Id)).Returns(user);
            A.CallTo(() => _userManager.ChangeEmailAsync(user, "new@mail.com", "token")).Returns(IdentityResult.Failed(new IdentityError()));

            var result = await _controller.ChangeEmail(user.Id, "new@mail.com", "token");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RequestChangeEmail_ReturnsOk_WhenSuccessful()
        {
            var user = new User { Id = "ischb8767t5r65techw7ey6t65", Email = "test@test.com" };
            SetUserContext(user.Id);

            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns(user);
            A.CallTo(() => _userManager.GenerateChangeEmailTokenAsync(user, "new@mail.com")).Returns("change-token");

            var result = await _controller.RequestChangeEmail(new RequestChangeEmailDTO { NewEmail = "new@mail.com" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("token", okResult.Value.ToString());
        }

        [Fact]
        public async Task RequestChangeEmail_ReturnsNotFound_WhenUserNotFound()
        {
            A.CallTo(() => _userManager.GetUserAsync(A<ClaimsPrincipal>._)).Returns((User)null);

            var result = await _controller.RequestChangeEmail(new RequestChangeEmailDTO { NewEmail = "new@mail.com" });

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }
    }
}