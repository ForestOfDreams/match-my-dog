using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Response
{
    public class Dog
    {
        public string Name { get; set; }

        public string Breed { get; set; }
        
        public double Weight { get; set; }

        public string OwnerUsername { get; set; }

        public static Dog FromDog(Models.User owner, Models.Dog dog) => new Dog()
        {
            Name = dog.Name,
            Breed = dog.Breed,
            Weight = dog.Weight,
            OwnerUsername = owner.Username
        };
    }
}
