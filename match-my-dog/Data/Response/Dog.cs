using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Response
{
    public class Dog
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public string Breed { get; set; }

        public double? Weight { get; set; }

        public bool Sex { get; set; }
        public DateTime Birthday { get; set; }

        public string OwnerUsername { get; set; }

        public string OwnerPhone { get; set; }

        public static Dog FromDog(Models.User owner, Models.Dog dog) => new Dog()
        {
            Id = dog.Id,
            Birthday = dog.Birthday,
            Name = dog.Name,
            Breed = dog.Breed,
            Weight = dog.Weight,
            Sex = dog.Sex,
            OwnerUsername = owner?.Username,
            OwnerPhone = owner?.Phone
        };
    }
}
