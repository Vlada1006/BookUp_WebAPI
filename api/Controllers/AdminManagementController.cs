using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.AdminManagement;
using api.Helpers;
using api.Interfaces;
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
        private readonly IAdminManagementService _adminManagementService;
        public AdminManagementController(IAdminManagementService adminManagementService)
        {
            _adminManagementService = adminManagementService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UsersQueryParameters queryParameters)
        {
            var users = await _adminManagementService.GetUsers(queryParameters);

            if (users == null)
            {
                return NotFound("Users not found");
            }

            return Ok(users.ToList());            
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await _adminManagementService.GetUserById(id);

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
            var user = await _adminManagementService.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> PartialUserUpdate([FromRoute] string id, [FromBody] JsonPatchDocument<UpdateUserDTO> patchDoc)
        {
            if (!ModelState.IsValid || patchDoc == null)
            {
                return BadRequest(ModelState);
            }

            var patchDTO = new UpdateUserDTO();

            patchDoc.ApplyTo(patchDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToUpdate = await _adminManagementService.PartialUserUpdate(id, patchDTO);

            if (userToUpdate == null)
            {
                return NotFound("User not found");
            }

            return Ok(userToUpdate.ToUserDto());
        }

        [HttpPost]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
           if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _adminManagementService.DeleteUser(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok("User deleted successfully");
        }
    }
}