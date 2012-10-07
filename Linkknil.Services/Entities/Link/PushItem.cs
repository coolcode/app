using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("Lnk_Push")]
    public class PushItem {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Url { get; set; }

        [MaxLength(100), Column(TypeName = "nvarchar")]
        public string Title { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string Target { get; set; }
        
        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Response { get; set; }


        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? PushTime { get; set; }

        //[Column(TypeName = "numeric(10,0)")]
        public int? TimeSpan { get; set; }
    }
}
