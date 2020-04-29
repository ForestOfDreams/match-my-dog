using match_my_dog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data
{
    public class UserData
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public static UserData FromUser(User user) => new UserData() {
            Name = user.Name,
            Username = user.Username,
            Role = user.Role
        };
    }
}
