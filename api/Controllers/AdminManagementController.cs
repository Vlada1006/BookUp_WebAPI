using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.AdminManagement;
using api.Mappers;
using api.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
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
        [Route("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }

        [HttpGet]
        [Route("email")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> PartialUserUpdate([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateUserDTO> updateUserDTO)
        {
//separate to service!!!
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User deleted successfully");
        }
    }
}