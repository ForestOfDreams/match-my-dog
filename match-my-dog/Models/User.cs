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
        [Column("username")]
        public string UserName { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
