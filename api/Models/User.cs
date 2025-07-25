using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class User : IdentityUser
    {
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DeletionWarningSent { get; set; }
    }
}