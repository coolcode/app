using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Web;
using Newtonsoft.Json;

namespace AliCould.com.API
{
    class Program
    {
        static void Main(string[] args)
        {
            string a = @"$%^";
            Console.WriteLine(HttpUtility.HtmlEncode(a));
            Console.WriteLine(HttpUtility.UrlEncode(a, 
                Encoding.UTF8));
            Console.WriteLine(HttpUtility.UrlEncodeUnicode(a));

            //var ja = new JArray(new string[] { "a", "b" });
            //JObject joo = JObject.Parse("{\"a\":\"b\"}");
            //Console.WriteLine(joo.ToString(Newtonsoft.Json.Formatting.None));

            //Console.ReadKey();

            //JObject resultDoc = new JObject();
            //resultDoc.Add("cmd", "action");
            //resultDoc["field"] = "asd";

            //NameValueCollection col = new NameValueCollection();
            //col.Add("a", "aa");
            //col.Add("b", "bb");
            
            //var jt = JToken.FromObject(col);
            
            //Console.WriteLine(jt.ToString());

            //CloudSearchHttpException testing = new CloudSearchHttpException("sdf", 4401);

            string clientId = "6100004160758124";
            string secretKey = "0007e8ebc3fefec7bd42d4d984db1cb5";
            string apiUrl = @"http://css.aliyun.com/v1/api";

            CloudSearchApi api = new CloudSearchApi(apiUrl, clientId, secretKey);
            var index = api.getIndex("testIndex");
            Console.WriteLine(index.getDocument("id_2"));

            JObject addingDocs = new JObject();
            addingDocs["cmd"] = "add";
            addingDocs["fields"] = generateDoc();

            index.addDocuments(addingDocs.ToString(Formatting.None));
            //int docCount = index.getTotalDocCount();
            //Console.WriteLine(docCount);

            //index.deleteDocuments("id_6");

            //string s = "abc";
            //string url = @"http://ali.contoso.com/xx/v1/api/";
            //Console.WriteLine(url.RTrim(@"/v1/api/"));

            //Console.WriteLine(Utilities.CalcMd5(s));
            //Console.WriteLine(DateTime.MinValue);
            //Console.WriteLine((DateTime.Now - DateTime.MinValue).TotalSeconds);
            //CloudSearchApi api = new CloudSearchApi("", "", "");
            //Console.WriteLine(CloudSearchApi.time());
            //var webReq = WebRequest.Create(@"http://css.aliyun.com/v1/api/index?nonce=89141b020411c061cf8545c50aa5c7df.1352443204&client_id=6100004160758124&sign=f77c35b25f19c60d3c0b074ec7390080");
            //webReq.Method = "GET";
            //var response = webReq.GetResponse();
            //StreamReader sr = new StreamReader(response.GetResponseStream());
            //var result = sr.ReadToEnd();

            //JObject jObj = JObject.Parse(result);
            //JToken errorMessage = null;
            //if (jObj.TryGetValue("errors", out errorMessage))
            //{
            //    Console.WriteLine(errorMessage[0]["code"]);
            //}
            //Console.WriteLine(result);
            Console.ReadKey();
        }

        static JObject generateDoc(string id = "", string title = "", string body = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "id_" + Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(title))
            {
                title = "title_" + Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(body))
            {
                body = "body_" + Guid.NewGuid().ToString();
            }

            JObject doc = new JObject();
            doc["id"] = id;
            doc["title"] = title;
            doc["body"] = body;

            return doc;
        }
    }
}
