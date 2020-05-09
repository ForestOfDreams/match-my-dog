using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace match_my_dog.Models
{
    [Table("users")]
    public class User
    {
        [Key, Column("id")]
        public long Id { get; set; }

        [Column("username"), Required]
        public string Username { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }

        [Column("password"), Required]
        public string Password { get; set; }

        [Column("role"), Required]
        public string Role { get; set; }
    }

    public static class Roles
    {
        public static string User => "user";
        public static string Administrator => "administrator";
    }
}
