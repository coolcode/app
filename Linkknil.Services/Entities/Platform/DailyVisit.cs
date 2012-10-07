using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("PF_DailyVisit")]
    public class DailyVisit {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string RefId { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string Type { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(10), Column(TypeName = "char")]
        public string DateText { get; set; }

        public int Count { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
