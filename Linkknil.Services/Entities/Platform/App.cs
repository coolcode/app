using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Linkknil.Entities {
    [Table("PF_App")]
    public class App {
        [Key]
        [Required]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string Id { get; set; }

       // [Association("FK_CategoryId", "CategoryId","Id")]
        [MaxLength(50), Column(TypeName = "varchar")]
        public string CategoryId { get; set; }

        public AppCategory Category { get; set; }

        [MaxLength(50), Column(TypeName = "varchar")]
        public string DeveloperId { get; set; }

        [Required]
        [MaxLength(30), Column(TypeName = "nvarchar")]
        public string Name { get; set; }

        [MaxLength(50), Column(TypeName = "nvarchar")]
        public string Intro { get; set; }

        [MaxLength(1000), Column(TypeName = "nvarchar")]
        public string Summary { get; set; }
        
        [MaxLength(50), Column(TypeName = "nvarchar")]
        public string Tag { get; set; }

        [MaxLength(1000), Column(TypeName = "varchar")]
        public string IconPath { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? AuditTime { get; set; }
        
        /// <summary>
        /// 审批人
        /// </summary>
        [MaxLength(50), Column(TypeName = "varchar")]
        public string AuditorId { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 用于管理员填写审核意见，跟应用开发商的交流
        /// </summary>
        [MaxLength(2000), Column(TypeName = "nvarchar")]
        public string Suggestion { get; set; }
    }
}
