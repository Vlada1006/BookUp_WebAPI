using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.Auth;
using api.Interfaces;
using api.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        public AuthController(UserManager<User> userManager, ITokenService tokenService,
                              SignInManager<User> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUserEmail = await _userManager.FindByEmailAsync(registerDTO.Email);

                if (existingUserEmail != null)
                {
                    return BadRequest(new { message = "Email is already registered." });
                }

                var existingUserName = await _userManager.FindByNameAsync(registerDTO.UserName);

                if (existingUserName != null)
                {
                    return BadRequest(new { message = "User Name is already taken." });
                }

                var newUser = new User
                {
                    UserName = registerDTO.UserName,
                    Email = registerDTO.Email
                };

                var createdUser = await _userManager.CreateAsync(newUser, registerDTO.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

                    if (!roleResult.Succeeded)
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedEmailToken = Uri.EscapeDataString(emailToken);
                var confirmationLink = $"http://localhost:5152/api/auth/confirm-email?userId={newUser.Id}&token={encodedEmailToken}";

                await _emailService.SendEmailAsync(
                    newUser.Email,
                    "Confirm your email",
                    $"Click the link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>"
                );

                return Ok(
                    new NewUserDTO
                    {
                        UserName = newUser.UserName,
                        Email = newUser.Email,
                        Token = _tokenService.CreateToken(newUser)
                    }
                );
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid email!");
            }

            var loginResult = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (!loginResult.Succeeded)
            {
                return Unauthorized("Email or/and password incorrect!");
            }

            return Ok(
                new NewUserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }
        
        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully");
            }

            return BadRequest("Invalid token or email confirmation failed");
        }
    }
}