using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using EaseEasy.ServiceModel.Services;
using EaseEasy.Data.Entity;
using Linkknil.Entities;
using Linkknil.Models;
using Nest;

namespace Linkknil.Services {
    public class SearchService : ServiceBase<LinkknilContext>, ISearchService {
        public void Index() {
            var sql =
                  @"select top 10 c.Id, c.Title,  c.Text, c.FriendlyHtml, c.EndTime as [PublishTime], a.Name as Author, a.Id as AppId, a.IconPath, c.ImagePath 
from lnk_content c
left join pf_app a on c.AppId = a.Id
where 1=1 ";

            var contents = db.Query<ContentViewModel>(sql);
             
            foreach (var item in contents) {
                Index(item);
                Console.WriteLine("Index:{0} on {1}",item.Title, DateTime.Now);
            }
        }

        public void Index<T>(T document) where T : class {
            var uriString = ConfigurationManager.AppSettings["SEARCHBOX_URL"];
            var searchBoxUri = new Uri(uriString);

            var elasticSettings = new ConnectionSettings(searchBoxUri)
                    .SetDefaultIndex("linkknil");

            var client = new ElasticClient(elasticSettings);

            client.Index(document);
        }

        public IEnumerable<ContentViewModel> Search(string keyword, int page = 0, int pageSize = 10) {

            var uriString = ConfigurationManager.AppSettings["SEARCHBOX_URL"];
            var searchBoxUri = new Uri(uriString);

            var elasticSettings = new ConnectionSettings(searchBoxUri)
                    .SetDefaultIndex("linkknil");

            var client = new ElasticClient(elasticSettings);

            var result = client.Search<ContentViewModel>(s => s
                                                .From(page)
                                                .Size(pageSize)
                //.Fields(f => f.Id, f => f.)
                                                .SortAscending(f => f.PublishTime)
                                                .Query(q => q.QueryString(qs => qs.Query(keyword))
                                                            ));

            return result.Documents;
        }
    }
}
