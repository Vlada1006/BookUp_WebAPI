using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.AdminManagement;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IAdminManagementService
    {
        public Task<List<UserDTO>> GetUsers(UsersQueryParameters queryParameters);
        public Task<User?> GetUserById(string id);
        public Task<User?> GetUserByEmail(string email);
        public Task<User?> PartialUserUpdate(string id, UpdateUserDTO updateUserDTO);
        public Task<User?> DeleteUser(string id);
    }
}