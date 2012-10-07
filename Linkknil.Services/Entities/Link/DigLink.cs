using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Linkknil.Entities {
    [Table("Lnk_DigLink")]
    public class DigLink {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string LinkId { get; set; }

        public LinkItem Link { get; set; }

        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

       // [Column(TypeName = "numeric(10,0)")]
        public int? TimeSpan { get; set; }

        public int DigNum { get; set; }
        public int FailNum { get; set; }
        public int DuplicateNum { get; set; }

        public int Status { get; set; }
    }
}
