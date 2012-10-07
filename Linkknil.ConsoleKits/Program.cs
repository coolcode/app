using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Linkknil.Services;
using NReadability;

namespace Linkknil.ConsoleKits
{
    class Program
    {
        class UrlPattern
        {
            public UrlPattern()
            {

            }

            public UrlPattern(string url, string pattern)
            {
                this.Url = url;
                this.Pattern = pattern;
            }

            public string Url { get; set; }
            public string Pattern { get; set; }
        }

        static void Main(string[] args)
        {
/*
            var rurl = "http://www.cnblogs.com/coolcode/archive/2012/09/26/beautiful_voice.html";
            var nReadabilityTranscoder = new NReadabilityWebTranscoder();
            var transResult = nReadabilityTranscoder.Transcode(new WebTranscodingInput(rurl));
            Console.WriteLine("Title:{0} \nContent:{1}",transResult.ExtractedTitle,transResult.ExtractedContent);

            if (transResult.ContentExtracted) {

                File.WriteAllText(
                  Path.Combine(Directory.GetCurrentDirectory(), string.Format("SampleOutput_{0:yyyyMMddHHmmss}.html", DateTime.Now)),
                  transResult.ExtractedContent,
                  Encoding.UTF8);
            }

            return;
            */
            var linkService = new LinkService();
            //linkService.PushLink();

            linkService.DigLink();

            return;
            //var links = linkService.QueryLinks();
            //foreach (var link in links)
            //{
            //    Console.WriteLine("{0}, {1}", link.Name, link.Status);
            //}

            var urlPullService = new PullService();
            var readabilityService = new ReadabilityService("coolcode", "csharp");

            int success = 0;
            int fail = 0;
            int duplicate = 0;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var urlPatterns = new List<UrlPattern>()
                                  {
/*ueo baidu*/
new UrlPattern("http://ueo.baidu.com","//div[@class=\"entry-title\"]/h2/a[@href]"),
/*36氪*/
new UrlPattern("http://www.36kr.com/category/us-startups","//*[@id=\"posts\"]/article/h3/a[@href]"),
new UrlPattern("http://www.36kr.com","//*[@id=\"posts\"]/article/h3/a[@href]"),
new UrlPattern("http://www.36kr.com/page/2","//*[@id=\"posts\"]/article/h3/a[@href]"),
                                      /* http://www.geekpark.net/read/seed */
/*爱范儿 艾饭尔*/
new UrlPattern("http://www.ifanr.com", "//div[@class=\"entry-header\"]/h2/a[@href]"),
/*人人都是产品经理 iamsujie 403 forbidden*/
//new UrlPattern("http://www.iamsujie.com/page/1", "//*[@id=\"main\"]/div[@class=\"post\"]/h2/a[@class=\"title\"][@href]"),
/*腾讯CDC*/
new UrlPattern("http://cdc.tencent.com","//div[@id=\"content\"]/div/div[@class=\"title\"]/h3/a[@href]"),
/*疯狂的设计 js 控制数据抓不到*/
//new UrlPattern("http://hi.baidu.com/new/madesign","//article/div/div[@class=\"mod-blogitem-title\"]/a[@href]"),
/*UCD China 用户研究*/
new UrlPattern("http://ucdchina.com/UR","//li[@class=\"post\"]/h4/a[@href]"),
/*UCD China 设计*/
new UrlPattern("http://ucdchina.com/UCD","//li[@class=\"post\"]/h4/a[@href]"),
/*UCD China 产品市场*/
new UrlPattern("http://ucdchina.com/PM","//li[@class=\"post\"]/h4/a[@href]"),

/* W3C */
new UrlPattern("http://www.w3cfuns.com/portal.php?mod=list&catid=18","//div[@id=\"wz_content\"]/ul/li/h1/a[@class=\"xi2\"][@href]"),
/*互联网一些事*/
new UrlPattern("http://www.yixieshi.com/pd","//div[@class=\"conList\"]/div/h2/a[@href]"),
new UrlPattern("http://www.yixieshi.com/it","//div[@class=\"conList\"]/div/h2/a[@href]"),

};

            foreach (var urlPattern in urlPatterns)
            {
                var sourceUrl = urlPattern.Url;
                var xpath = urlPattern.Pattern;


                var ur = new Uri(sourceUrl);
                var mainUrl = string.Format("{0}://{1}:{2}", ur.Scheme, ur.Host, ur.Port);

                var urls = urlPullService.PullLink(sourceUrl, xpath);
                var result = PushToReadability(urls.Select(c=>c.Url), mainUrl, readabilityService);

                success += result.SuccessCount;
                fail += result.FailCount;
                duplicate += result.DuplicateCount;

                if(result.FailCount>0)
                {
                    Console.WriteLine("重试推送下列url：");
                    foreach (var url in result.FailUrls)
                    {
                        Console.WriteLine(url);
                    }

                    result = PushToReadability(result.FailUrls, mainUrl, readabilityService);

                    success += result.SuccessCount;
                    fail -= result.SuccessCount;
                    duplicate += result.DuplicateCount;
                }
            }

            stopWatch.Stop();

            Console.WriteLine("finish. Time:{0}, success:{1}, fail:{2}, duplicate:{3}", stopWatch.Elapsed, success, fail, duplicate);
            Console.Read();
        }

        private static PushResult PushToReadability(IEnumerable<string> urls, string mainUrl, ReadabilityService readabilityService)
        {
            var pushResult = new PushResult();

            foreach (var url in urls)
            {
                try
                {
                    var u = url;
                    if (!url.StartsWith("http:"))
                    {
                        u = mainUrl + "/" + url;
                    }

                    var result = readabilityService.Bookmark(u);
                    Console.WriteLine("Success To Read:{0}", u);
                    pushResult.SuccessCount ++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fail To Read:{0},{1}", url, ex.Message);

                    if (ex.Message.Contains("(404)"))
                    {
                        pushResult.FailUrls.Add(url);
                    }
                    else if (ex.Message.Contains("(409)"))
                    {
                        pushResult.DuplicateUrls.Add(url);
                    } 
                    
                }
            }

            return pushResult;
        }
    }

    public class PushResult
    {
        public int SuccessCount { get; set; }
        public int FailCount { get { return FailUrls.Count; } }
        public int DuplicateCount { get { return DuplicateUrls.Count; } }
        public IList<string> FailUrls { get; private set; }
        public IList<string> DuplicateUrls { get;private set; }

        public PushResult()
        {
            FailUrls = new List<string>();
            DuplicateUrls =new List<string>();
        }
    }
}
