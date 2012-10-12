using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Linkknil.Services {
    public class RssService {
        public IEnumerable<Link> PullLink(string url) {
            XmlReader reader = XmlReader.Create(url);

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            if (feed == null) {
                return Enumerable.Empty<Link>();
            }

            var list = new List<Link>();
            foreach (SyndicationItem item in feed.Items) {
                var rsslink = item.Links.FirstOrDefault();
                if (rsslink == null) {
                    continue;
                }

                list.Add(new Link { Title = item.Title.Text, Url = rsslink.Uri.ToString() });
            }

            reader.Close();

            return list;
        }
    }
}
