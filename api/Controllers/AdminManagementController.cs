using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/adminManagement")]
    // [Authorize(Roles = "Admin")]
    public class AdminManagementController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public AdminManagementController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null)
            {
                return NotFound("Users not found");
            }

            var usersDTO = users.Select(u => u.ToUserDto()).ToList();

            return Ok(usersDTO);
        }

        [HttpGet]
        [Route("{id}:string")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }
    }
}