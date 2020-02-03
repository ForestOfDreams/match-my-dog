using match_my_dog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Services
{
    public class ProfileRepository
    {
        public Profile[] GetAllProfiles()
        {
            return new[] {
                new Profile()
                {
                    Id = 0,
                    Name = "Vasya",
                    Sex = true
                },
                new Profile()
                {
                    Id = 1,
                    Name = "Nastya",
                    Sex = false
                }

            };
        }
    }
}
