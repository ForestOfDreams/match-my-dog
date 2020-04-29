using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using match_my_dog.Models;
using Microsoft.AspNetCore.Authorization;
using match_my_dog.Data;

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
        public async Task<ActionResult<UserData>> GetMe()
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            return UserData.FromUser(user);
        }

        private User GetUser() => context.Users.FirstOrDefault(user => user.Username == User.Identity.Name);

        [Authorize]
        [HttpPost("me")]
        public async Task<ActionResult> PostMe(UserEditData userEditData)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            user.Name = userEditData.Name;
            context.SaveChanges();

            return Ok();
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<ActionResult> DeleteMe()
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            context.Remove(user);
            context.SaveChanges();

            return Ok();
        }
    }
}
