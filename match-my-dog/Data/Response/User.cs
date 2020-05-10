using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Data.Response
{
    public class User
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }

        public static User FromUser(Models.User user) => new User()
        {
            Phone = user.Phone,
            Name = user.Name,
            Username = user.Username,
            Role = user.Role
        };
    }
}
