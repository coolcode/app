using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("Lnk_Link")]
    public class LinkItem {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string AppId { get; set; }

        public App App { get; set; }

        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Url { get; set; }

        [MaxLength(255), Column(TypeName = "varchar")]
        public string XPath { get; set; }

        //抓取类型：网页xpath、rss
        [MaxLength(20), Column(TypeName = "varchar")]
        public string PullType{ get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? HandleTime { get; set; }
    }
}
