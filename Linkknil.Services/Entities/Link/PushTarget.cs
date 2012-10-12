using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("Lnk_PushTarget")]
    public class PushTarget {

        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(50), Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string UserId { get; set; }

        [MaxLength(255), Column(TypeName = "varchar")]
        public string Account { get; set; }

        [MaxLength(255), Column(TypeName = "varchar")]
        public string Password { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
