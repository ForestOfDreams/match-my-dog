﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Request.Dog
{
    public class Post
    {

        public string Name { get; set; }

        public string Breed { get; set; }

        public double? Weight { get; set; }

        public bool Sex { get; set; }
        public string? Birthday { get; set; }
    }
}
