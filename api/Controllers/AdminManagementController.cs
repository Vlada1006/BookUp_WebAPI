using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/adminManagement")]
    [Authorize(Roles = "Admin")]
    public class AdminManagementController : ControllerBase
    {

    }
}