﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Request.Dog.My
{
    public class Put
    {
        public string Name { get; set; }

        public string Breed { get; set; }

        public double Weight { get; set; }
    }
}
