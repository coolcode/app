using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linkknil.Entities {
    public class ContentViewModel {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime PublishTime { get; set; }
        public string AppId { get; set; }
        public string Author { get; set; }
        public string PublishTimeText { get { return PublishTime.ToString("yyyy-MM-dd HH:mm"); } }

        public string IconPath { get; set; }
        public string ImagePath { get; set; }
        
        public string Text { get; set; }
        public string FriendlyHtml { get; set; }
    }
}
