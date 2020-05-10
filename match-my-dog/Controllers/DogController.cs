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
        [HttpGet()]
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
        [HttpPost("{id}")]
        public async Task<ActionResult> PostMyDog(long id, Data.Request.Dog.Post data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId);

            dog.Name = data.Name;
            dog.Weight = data.Weight;
            dog.Breed = data.Breed;
            dog.Sex = data.Sex;
            dog.Birthday = data.Birthday;

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPut()]
        public async Task<ActionResult> PutMyDog(Data.Request.Dog.Put data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            context.Dogs.Add(new Models.Dog() { Name = data.Name, Breed = data.Breed, Weight = data.Weight, Birthday = data.Birthday, OwnerId = user.Id });

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMyDog(long id)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId);

            context.Dogs.Remove(dog);

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}