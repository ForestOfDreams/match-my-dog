using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using match_my_dog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace match_my_dog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DogController : ControllerBase
    {
        private readonly DatabaseContext context;

        public DogController(DatabaseContext context)
        {
            this.context = context;
        }
        private User GetUser() => long.TryParse(User.Identity.Name, out long id) ? context.Users.FirstOrDefault(user => user.Id == id) : null;

        [Authorize]
        [HttpGet("my")]
        public async Task<ActionResult<Data.Response.Dog[]>> GetMyDog()
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            return context.Dogs
                .Where(dog => dog.OwnerId == user.Id)
                .Select(dog => Data.Response.Dog.FromDog(user, dog))
                .ToArray();
        }
    }
}