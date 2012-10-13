using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("Lnk_Content")]
    public class Content {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string LinkId { get; set; }

        public LinkItem Link { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string AppId { get; set; }

        public App App { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string DeveloperId { get; set; }

        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Url { get; set; }

        [MaxLength(100), Column(TypeName = "nvarchar")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Html { get; set; }

        [MaxLength(100), Column(TypeName = "nvarchar")]
        public string FriendlyTitle { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string FriendlyHtml { get; set; }

        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Response { get; set; }

        [MaxLength(50), Column(TypeName = "nvarchar")]
        public string Tag { get; set; }

        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string ImagePath { get; set; }

        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }

        //[Column(TypeName = "numeric(10,0)")]
        public int? TimeSpan { get; set; }
    }
}
