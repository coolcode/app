using AliCould.com.API;
using EaseEasy.Data.Entity;
using EaseEasy.ServiceModel.Services;
using Linkknil.Entities;
using Linkknil.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Linkknil.Services {
    public class AliyunSearchService : ServiceBase<LinkknilContext>, ISearchService {

        const string url = "http://css.aliyun.com/v1/api";

        //const string client_id = "6100947599546028";
        //const string secret_id = "0f0ad4259056eb4dc1690e7ccd1aaedc";

        const string client_id = "6100658993139617";
        const string secret_id = "517de3c3e8e055cbf8810063658b3771";

        /*6100658993139617
    密钥：	517de3c3e8e055cbf8810063658b3771*/
        private CloudSearchApi api = new CloudSearchApi(url, client_id, secret_id);

        public void Index() {
            var sql =
                @"select top 30 c.Id, c.Title,  c.Text, c.FriendlyHtml, c.EndTime as [PublishTime], a.Name as Author, a.Id as AppId, a.IconPath, c.ImagePath 
from lnk_content c
left join pf_app a on c.AppId = a.Id
where 1=1 ";
            
            var contents = db.Query<ContentViewModel>(sql);


            api.createIndex("posts2", "news");
            var index = api.getIndex("posts2");
            List<string> docs = new List<string>();
            foreach (var document in contents) {

                var doc = new JObject();
                doc["cmd"] = "add";
                doc["fields"] = new JObject();
                doc["fields"]["id"] = document.Id;
                doc["fields"]["title"] = document.Title;
                doc["fields"]["body"] = document.Text;
                var d = doc.ToString(Newtonsoft.Json.Formatting.None);
                docs.Add(d);
                Console.WriteLine("Index:{0} on {1}", document.Title, DateTime.Now);
            //    index.addDocument(d);
            }

            index.addDocuments(docs.ToArray());
            
        }
 
        public IEnumerable<ContentViewModel> Search(string keyword, int page = 0, int pageSize = 10) {

            CloudSearchIndex index = api.getIndex("posts2");
            string searchResult = index.search("q=" + keyword, (page + 1).ToString(), pageSize.ToString(), null, null, null, null);
            JObject sr = JObject.Parse(searchResult);

            if (sr["result"] != null) {
                JArray resultItems = sr["result"]["items"] as JArray;
                if (resultItems != null) {
                    string[] fields = new string[] { "id", "type_id", "cat_id", "title", "body", "author" };
                  
                    foreach (JObject jo in resultItems) {
                        var content = new ContentViewModel();
                        content.Id = jo["id"].Value<string>();
                        content.Title = jo["title"].Value<string>();
                        content.Text = jo["body"].Value<string>();

                        yield return content;
                    }

                }
            }
        }
    }
}
