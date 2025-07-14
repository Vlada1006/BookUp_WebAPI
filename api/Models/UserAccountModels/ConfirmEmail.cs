using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.OtherModels.UserAccountModels
{
    public class ConfirmEmail
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}