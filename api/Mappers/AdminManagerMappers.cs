using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.DTOs.AdminManagement;
using api.DTOs.Auth;
using api.Models;

namespace api.Mappers
{
    public static class AdminManagerMappers
    {
        public static UserDTO ToUserDto(this User userModel)
        {
            return new UserDTO
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email,
                LastLoginDate = userModel.LastLoginDate.HasValue
                    ? userModel.LastLoginDate.Value.ToString("yyyy-MM-dd HH:mm") 
                    : "wasn`t logged in"
            };
        }
    }
}