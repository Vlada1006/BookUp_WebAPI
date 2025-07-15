using System.Net;
using System.Security.Claims;
using api.DTOs.UserAccount;
using api.Interfaces;
using api.Models;
using api.Models.DTOs.UserAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/userAccount")]
    public class UserAccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        public UserAccountController(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }


        [HttpGet]
        [Route("account-data")]
        public async Task<IActionResult> GetUserAccountData(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = new
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);

            if (changePasswordDTO.CurrentPassword == changePasswordDTO.NewPassword)
            {
                return BadRequest("New password can be the same as old one");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password changed successfully");
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var decodedToken = WebUtility.UrlDecode(resetPasswordDTO.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password reset successfully");
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

            if (user == null)
            {
                return Ok("If your email is registered, a reset link will be sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedEmailToken = WebUtility.UrlEncode(token);

            return Ok(new { Token = encodedEmailToken });
        }

        [HttpPost]
        [Route("change-email")]
        public async Task<IActionResult> ChangeEmail(string userId, string newEmail, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var decodedToken = Uri.UnescapeDataString(token);

            var result = await _userManager.ChangeEmailAsync(user, newEmail, decodedToken);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Uri.EscapeDataString(emailToken);
            var confirmationLink = $"http://localhost:5152/api/auth/confirm-email?userId={user.Id}&token={encodedEmailToken}";

            await _emailService.SendEmailAsync(
                newEmail,
                "Confirm your email",
                $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>"
            );

            return Ok("Email changed successfully. Please confirm your new email.");
        }

        [HttpPost]
        [Route("request-change-email")]
        public async Task<IActionResult> RequestChangeEmail([FromBody] RequestChangeEmailDTO requestChangeEmailDTO)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, requestChangeEmailDTO.NewEmail);
            var encodedEmailToken = Uri.EscapeDataString(token);

            var confirmationLink = $"http://localhost:5152/api/auth/change-email-confirm?userId={user.Id}&newEmail={requestChangeEmailDTO.NewEmail}&token={encodedEmailToken}";

            await _emailService.SendEmailAsync(
                requestChangeEmailDTO.NewEmail,
                 "Confirm your new email",
                $"Click this link to confirm your new email: <a href='{confirmationLink}'>Confirm Email</a>"
            );

            return Ok(new
            {
                message = "Confirmation email sent to new address.",
                token = encodedEmailToken,
                confirmationLink
            });
        }
    }
}