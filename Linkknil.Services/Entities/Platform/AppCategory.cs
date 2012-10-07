using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("PF_AppCategory")]
    public class AppCategory {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(20), Column(TypeName = "varchar")]
        public string Sort { get; set; }

        [Required]
        [MaxLength(20), Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        public int Status { get; set; }
    }
}
