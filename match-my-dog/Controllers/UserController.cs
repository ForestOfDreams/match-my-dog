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
using System.Text.RegularExpressions;

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

        [HttpGet("{username}")]
        public async Task<ActionResult<Data.Response.User>> GetUser(string username)
        {
            var user = context.Users.FirstOrDefault(user => user.Username == username);

            if (user == null) return BadRequest(Error.BadUserId());

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

            if (!CheckPhone(data.Phone)) return BadRequest(Error.BadPhone());

            user.Name = data.Name;
            user.Phone = data.Phone;

            await context.SaveChangesAsync();

            return Ok();
        }

        public static Regex phoneRegex = new Regex(@"^(\+7|7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$");

        public static bool CheckPhone(string phone) => phoneRegex.IsMatch(phone);

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
