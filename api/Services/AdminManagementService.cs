using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.AdminManagement;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class AdminManagementService : IAdminManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public AdminManagementService(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }


        public async Task<List<UserDTO>> GetUsers(UsersQueryParameters queryParameters)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParameters.NameSearchTerm))
            {
                users = users.Where(u => u.UserName.ToLower().Contains(queryParameters.NameSearchTerm.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(queryParameters.EmailSearchTerm))
            {
                users = users.Where(u => u.Email.ToLower().Contains(queryParameters.EmailSearchTerm.ToLower()));
            }

            switch (queryParameters.SortBy)
            {
                case "userName":
                    users = !queryParameters.IsDescending ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName);
                    break;
                case "email":
                    users = !queryParameters.IsDescending ? users.OrderBy(u => u.Email) : users.OrderByDescending(u => u.Email);
                    break;
            }

            users = users.Skip(queryParameters.Size * (queryParameters.Page - 1)).Take(queryParameters.Size);

            var usersToReturn = await users.ToListAsync();
            return usersToReturn.Select(u => u.ToUserDto()).ToList();
        }


        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> PartialUserUpdate(string id, UpdateUserDTO patchDTO)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var currentEmail = user.Email;

            if (patchDTO.UserName != null) user.UserName = patchDTO.UserName;
            if (patchDTO.Email != null) user.Email = patchDTO.Email;
            if (patchDTO.PhoneNumber != null) user.PhoneNumber = patchDTO.PhoneNumber;

            if (currentEmail != user.Email)
            {
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedEmailToken = Uri.EscapeDataString(emailToken);
                var confirmationLink = $"http://localhost:5152/api/auth/confirm-email?userId={user.Id}&token={encodedEmailToken}";

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Confirm your email",
                    $"Your email was changed by out Administration by request. Click the link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>"
                );
            }

            return user;
        }

        public async Task<User?> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return null;
            }

            return user;
        }

      
    }
}