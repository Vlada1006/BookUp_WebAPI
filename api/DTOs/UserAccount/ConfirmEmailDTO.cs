using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.DTOs.UserAccount
{
    public class ConfirmEmailDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}