using System;
using System.Linq;
using System.Threading.Tasks;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using match_my_dog.Data.Request.Dog;
using match_my_dog.Data.Response;
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
        private readonly ImgurClient imgur = new ImgurClient(Config.ImgurClientId, Config.ImgurClientSecret);

        public DogController(DatabaseContext context)
        {
            this.context = context;
        }
        private Models.User GetUser() => long.TryParse(User.Identity.Name, out long id) ? context.Users.FirstOrDefault(user => user.Id == id) : null;

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Response.Dog>> GetDog(long id)
        {
            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id);

            if (dog == null) return BadRequest(Error.BadDogId());

            var user = context.Users.FirstOrDefault(user => user.Id == dog.OwnerId);

            if (user == null) return UnprocessableEntity();

            return Data.Response.Dog.FromDog(user, dog);
        }

        [Authorize]
        [HttpGet]
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

            if (dog == null) return BadRequest(Error.BadDogId());

            if (!CheckWeight(data.Weight ?? 0)) return BadRequest(Error.BadWeight());

            if (!CheckBirthday(data.Birthday)) return BadRequest(Error.BadBirthday());

            dog.Name = data.Name;
            dog.Weight = data.Weight;
            dog.Breed = data.Breed;
            dog.Sex = data.Sex;
            dog.Birthday = data.Birthday;

            await context.SaveChangesAsync();

            return Ok();
        }

        private static bool CheckWeight(double weight) => weight > 0 || weight < 150;

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> PutMyDog(Put data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            if (!CheckWeight(data.Weight ?? 0)) return BadRequest(Error.BadWeight());

            if (!CheckBirthday(data.Birthday)) return BadRequest(Error.BadBirthday());

            context.Dogs.Add(new Models.Dog() { Name = data.Name, Breed = data.Breed, Weight = data.Weight, Birthday = data.Birthday, OwnerId = user.Id });

            await context.SaveChangesAsync();

            return Ok();
        }

        private static bool CheckBirthday(DateTime birthday)
        {
            return birthday < DateTime.Now.AddYears(-100) || birthday > DateTime.Now;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutMyDog(long id, IFormFile image)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId());

            if (image == null)
            {
                dog.Avatar = null;
                await context.SaveChangesAsync();
            }
            else
            {
                if (image.ContentType.Split('/').FirstOrDefault() != "image") return BadRequest(Error.FileUploadError("Bad content type"));

                try
                {
                    var endpoint = new ImageEndpoint(imgur);
                    var result = await endpoint.UploadImageStreamAsync(image.OpenReadStream());

                    dog.Avatar = result.Link;

                    await context.SaveChangesAsync();
                }
                catch (ImgurException ex)
                {
                    return BadRequest(Error.FileUploadError(ex.Message));
                }
            }

            return Ok();
        }



        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMyDog(long id)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var dog = context.Dogs.FirstOrDefault(dog => dog.Id == id && dog.OwnerId == user.Id);

            if (dog == null) return BadRequest(Error.BadDogId());

            context.Dogs.Remove(dog);

            await context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("find")]
        public async Task<ActionResult<Data.Response.Dog[]>> FindDog(Data.Request.Dog.Find.Post data)
        {
            var user = GetUser();

            if (user == null) return Unauthorized();

            var breed = data.Breed.ToLower().Trim();
            var birthdayMax = DateTime.Now.AddYears(-data.AgeMin ?? 0);
            var birthdayMin = DateTime.Now.AddYears(-data.AgeMax ?? 0);

            return context.Dogs
                .Where(
                    dog => dog.OwnerId != user.Id && 
                    dog.Sex == data.Sex &&
                    (dog.Breed.ToLower().Trim().Contains(breed) || breed.Contains(dog.Breed.ToLower().Trim())) &&
                    (data.WeightMin == null || dog.Weight > data.WeightMin) &&
                    (data.WeightMax == null || dog.Weight < data.WeightMax) &&
                    (data.AgeMin == null ||  dog.Birthday < birthdayMax) &&
                    (data.AgeMax == null || dog.Birthday > birthdayMin)
                )
                .Select(dog => Data.Response.Dog.FromDog(context.Users.FirstOrDefault(user => user.Id == dog.OwnerId), dog))
                .ToArray();
        }
    }
}