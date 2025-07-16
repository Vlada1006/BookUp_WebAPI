using api.Controllers;
using api.DTOs.Auth;
using api.Interfaces;
using api.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using MockQueryable;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace BookUp.UnitTests.ControllerTests
{
    public class AuthControllerTests
    {
        private readonly UserManager<User> _fakeUserManager;
        private readonly IEmailService _fakeEmailService;
        private readonly ITokenService _fakeTokenService;
        private readonly SignInManager<User> _fakeSignInManager;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            // Mock dependencies
            var userStore = A.Fake<IUserStore<User>>();
            _fakeUserManager = A.Fake<UserManager<User>>(options =>
                options.WithArgumentsForConstructor(() => new UserManager<User>(
                    userStore, null, null, null, null, null, null, null, null)));

            var contextAccessor = A.Fake<IHttpContextAccessor>();
            var userPrincipalFactory = A.Fake<IUserClaimsPrincipalFactory<User>>();

            _fakeSignInManager = A.Fake<SignInManager<User>>(options =>
                options.WithArgumentsForConstructor(() => new SignInManager<User>(
                    _fakeUserManager, contextAccessor, userPrincipalFactory, null, null, null, null)));

            _fakeEmailService = A.Fake<IEmailService>();
            _fakeTokenService = A.Fake<ITokenService>();

            _controller = new AuthController(_fakeUserManager, _fakeTokenService, _fakeSignInManager, _fakeEmailService);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
        {
            var registerDto = new RegisterDTO
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test123!"
            };

            A.CallTo(() => _fakeUserManager.FindByEmailAsync(registerDto.Email)).Returns((User)null);
            A.CallTo(() => _fakeUserManager.FindByNameAsync(registerDto.UserName)).Returns((User)null);
            A.CallTo(() => _fakeUserManager.CreateAsync(A<User>._, registerDto.Password)).Returns(IdentityResult.Success);
            A.CallTo(() => _fakeUserManager.AddToRoleAsync(A<User>._, "User")).Returns(IdentityResult.Success);
            A.CallTo(() => _fakeUserManager.GenerateEmailConfirmationTokenAsync(A<User>._)).Returns("fake-token");
            A.CallTo(() => _fakeTokenService.CreateToken(A<User>._)).Returns("fake-jwt-token");

            var result = await _controller.Register(registerDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var newUserDto = Assert.IsType<NewUserDTO>(okResult.Value);

            Assert.Equal(registerDto.UserName, newUserDto.UserName);
            Assert.Equal(registerDto.Email, newUserDto.Email);
            Assert.Equal("fake-jwt-token", newUserDto.Token);

            A.CallTo(() => _fakeEmailService.SendEmailAsync(registerDto.Email, A<string>._,
                A<string>.That.Contains("Confirm Email"))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var loginDTO = new LoginDTO
            {
                Email = "test@test.com",
                Password = "Test123!"
            };

            var testUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testUser",
                Email = "test@test.com"
            };

            A.CallTo(() => _fakeUserManager.Users).Returns(new List<User> { testUser }.AsQueryable().BuildMock());

            A.CallTo(() => _fakeSignInManager.CheckPasswordSignInAsync(testUser, loginDTO.Password, false)).Returns(SignInResult.Success);

            A.CallTo(() => _fakeTokenService.CreateToken(testUser)).Returns("fake-jwt-token");

            A.CallTo(() => _fakeUserManager.UpdateAsync(testUser)).Returns(IdentityResult.Success);

            var result = await _controller.Login(loginDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var newUserDto = Assert.IsType<NewUserDTO>(okResult.Value);

            Assert.Equal(testUser.UserName, newUserDto.UserName);
            Assert.Equal(testUser.Email, newUserDto.Email);
            Assert.Equal("fake-jwt-token", newUserDto.Token);

            A.CallTo(() => _fakeUserManager.UpdateAsync(testUser)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsOk_WhenConfirmationSucceeds()
        {
            var userId = "ych873t6fceijncehfn";
            var token = "valid-token";
            var user = new User { Id = userId, Email = "test@test.com" };

            A.CallTo(() => _fakeUserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _fakeUserManager.ConfirmEmailAsync(user, token)).Returns(IdentityResult.Success);

            var result = await _controller.ConfirmEmail(userId, token);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Email confirmed successfully", okResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var userId = "non-existent";
            var token = "token";

            A.CallTo(() => _fakeUserManager.FindByIdAsync(userId)).Returns((User)null);

            var result = await _controller.ConfirmEmail(userId, token);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task ConfirmEmail_ReturnsBadRequest_WhenConfirmationFails()
        {
            var userId = "user123";
            var token = "invalid-token";
            var user = new User { Id = userId, Email = "test@test.com" };

            A.CallTo(() => _fakeUserManager.FindByIdAsync(userId)).Returns(user);
            A.CallTo(() => _fakeUserManager.ConfirmEmailAsync(user, token)).Returns(IdentityResult.Failed());

            var result = await _controller.ConfirmEmail(userId, token);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid token or email confirmation failed", badRequestResult.Value);
        }
    }
}
