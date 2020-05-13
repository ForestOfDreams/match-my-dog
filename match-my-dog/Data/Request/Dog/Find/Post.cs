using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Request.Dog.Find
{
    public class Post
    {
        public bool Sex { get; set; }
        public string Breed { get; set; }
        public double? WeightMin { get; set; }
        public double? WeightMax { get; set; }
    }
}
