using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using match_my_dog.Models;
using Microsoft.AspNetCore.Authorization;

namespace match_my_dog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext context;

        public UserController(DatabaseContext context)
        {
            this.context = context;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            var user = context.Users.FirstOrDefault(user => user.Username == User.Identity.Name);

            if (user == null) BadRequest();

            return user;
        }
    }
}
