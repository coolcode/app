using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace Linkknil.Services {
    public class PullService {
        public IEnumerable<Link> PullLink(string url, string xpath) {
            var html = PullHtml(url);
            //根据xpath从html查找链接
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var linkNodes = doc.DocumentNode.SelectNodes(xpath);

            if (linkNodes == null || linkNodes.Count == 0) {
                throw new Exception(string.Format("Html找不到链接。地址：{0}, 模板：{1}, Html:{2}", url, xpath, html));
            }

            //从Nodes获取href、title属性

            return (from link in linkNodes
                    let att = link.Attributes["href"]
                    select new Link { Url = att.Value, Title = link.InnerText });
        }

        public string PullHtml(string url) {
            var client = new HttpClient();
            var urlStreamTask = client.GetStreamAsync(url);
            var responseTask = urlStreamTask.ContinueWith(response => {
                if (response.Exception != null) {
                    throw response.Exception;
                }

                using (var memoryStream = new MemoryStream()) {
                    response.Result.CopyTo(memoryStream);
                    //默认按utf8编码读取html
                    string html = ReadHtml(memoryStream, Encoding.UTF8);

                    //根据html的charset判断编码类型
                    Match charSetMatch = Regex.Match(html, "charset=(?<code>[a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase);
                    string chartSet = charSetMatch.Groups["code"].Value;
                    if (!string.IsNullOrEmpty(chartSet) && !chartSet.Equals("utf-8", StringComparison.OrdinalIgnoreCase)) {
                        html = ReadHtml(memoryStream, Encoding.GetEncoding(chartSet));
                    }

                    return html;
                }
            });

            return responseTask.Result;
        }

        private string ReadHtml(Stream stream, Encoding encoding) {
            stream.Position = 0;
            var reader = new StreamReader(stream, encoding);

            return reader.ReadToEnd();
        }
    }

}
