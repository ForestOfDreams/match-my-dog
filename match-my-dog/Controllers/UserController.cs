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
using match_my_dog.Data.Response;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Response.User>> GetUser(long id)
        {
            var user = context.Users.FirstOrDefault(user => user.Id == id);

            if (user == null) return BadRequest(Error.BadUserId);

            return Data.Response.User.FromUser(user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Data.Response.User>> GetMe()
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            return Data.Response.User.FromUser(user);
        }

        private Models.User GetUser() => long.TryParse(User.Identity.Name, out long id) ? context.Users.FirstOrDefault(user => user.Id == id) : null;

        [Authorize]
        [HttpPost("me")]
        public async Task<ActionResult> PostMe(Data.Request.User.Me.Post data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            user.Name = data.Name;
            user.Phone = data.Phone;

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("me")]
        public async Task<ActionResult> DeleteMe()
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            context.Remove(user);
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
