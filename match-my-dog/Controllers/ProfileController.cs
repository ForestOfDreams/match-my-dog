using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using match_my_dog.Models;
using match_my_dog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace match_my_dog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private ProfileRepository profileRepository = new ProfileRepository();

        // GET: api/Profile
        [HttpGet]
        public IActionResult Get()
        {
            // return profile of user
            return Ok(profileRepository.GetAllProfiles()[0]);
        }

        // GET: api/Profile/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var users = profileRepository.GetAllProfiles();
            if (id >= users.Length)
                return ValidationProblem();

            return Ok(users[id]);
        }

        // POST: api/Profile
        [HttpPost]
        public IActionResult Post([FromBody] ProfileUpdate value)
        {
            // update user profile
            Console.WriteLine(value.Name);

            if (new Random().NextDouble() > 0.5)
                return ValidationProblem();

            return Ok();

        }

        // DELETE: api/ApiWithActions/5
        /*[HttpDelete]
        public void Delete(int id)
        {
            // remove user profile
        }*/
    }
}
