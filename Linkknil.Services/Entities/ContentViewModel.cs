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
        public string Author { get; set; }
        public string PublishTimeText { get { return PublishTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        public string IconPath { get; set; }
    }
}
