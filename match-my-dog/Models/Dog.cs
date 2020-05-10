using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace match_my_dog.Models
{
    [Table("dogs")]
    public class Dog
    {
        [Key, Column("id")]
        public long Id { get; set; }

        [ForeignKey("owner_id"), Column("owner_id"), Required]
        public long OwnerId { get; set; }

        [Column("name"), Required]
        public string Name { get; set; }

        [Column("breed"), Required]
        public string Breed { get; set; }

        [Column("weight")]
        public double? Weight { get; set; }

        [Column("sex"), Required]
        public bool Sex { get; set; }

        [Column("birthday")]
        public DateTime Birthday { get; set; }
    }
}
