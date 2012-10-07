using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    public abstract class BaseDeveloper {

        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [Required]
        [MaxLength(20), Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        public int Status { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string Passport { get; set; }

        [MaxLength(255), Column(TypeName = "varchar")]
        public string Mail { get; set; }

        [MaxLength(20), Column(TypeName = "varchar")]
        public string Mobile { get; set; }

        [MaxLength(20), Column(TypeName = "varchar")]
        public string QQ { get; set; }

        [MaxLength(255), Column(TypeName = "varchar")]
        public string Site { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
