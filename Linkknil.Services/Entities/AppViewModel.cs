using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkknil.Entities {
    public class AppViewModel{
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string DeveloperId { get; set; }

        public string Name { get; set; }

        public string Intro { get; set; }
        public string Summary { get; set; }

        public string Tag { get; set; }

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
        public string AuditorId { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime? PublishTime { get; set; }

        /// <summary>
        /// 用于管理员填写审核意见，跟应用开发商的交流
        /// </summary>
        public string Suggestion { get; set; }
        public string Dev { get; set; }
        public int VisitNum { get; set; }
    }
}
