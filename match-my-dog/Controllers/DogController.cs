using System.Linq;
using System.Threading.Tasks;
using match_my_dog.Data.Response;
using match_my_dog.Models;
using Microsoft.AspNetCore.Authorization;
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
        private Models.User GetUser() => long.TryParse(User.Identity.Name, out long id) ? context.Users.FirstOrDefault(user => user.Id == id) : null;

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Response.Dog>> GetDog(long id)
        {
            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id);

            if (dog == null) return BadRequest(Error.BadDogId);

            var user = context.Users.FirstOrDefault(user => user.Id == dog.OwnerId);

            if (user == null) return UnprocessableEntity();

            return Data.Response.Dog.FromDog(user, dog);
        }

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

        [Authorize]
        [HttpPost("my")]
        public async Task<ActionResult> PostMyDog(Data.Request.Dog.My.Post data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == data.Id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId);

            dog.Name = data.Name;
            dog.Weight = data.Weight;
            dog.Breed = data.Breed;

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut("my")]
        public async Task<ActionResult> PutMyDog(Data.Request.Dog.My.Put data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            context.Dogs.Add(new Models.Dog() { Name = data.Name, Breed = data.Breed, Weight = data.Weight, OwnerId = user.Id });

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("my")]
        public async Task<ActionResult> DeleteMyDog(Data.Request.Dog.My.Delete data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == data.Id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId);

            context.Dogs.Remove(dog);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}