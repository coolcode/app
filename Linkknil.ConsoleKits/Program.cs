using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Linkknil.Services;
using Linkknil.StreamStore;
using NReadability;
using Linkknil.Models;
using EaseEasy.Data.Entity;

namespace Linkknil.ConsoleKits {
    class Program {

        static void Main(string[] args) {
            Stopwatch watch = Stopwatch.StartNew();

            //RssServiceTest();
            //SearchServiceTest();
            //  AliyunFileServiceTest();
            // AliyunSearchServiceTest();
            InitAppLink();
           // ReadabilityTest();
            //AliyunMultiObjectServiceTest();

            //var store = new NavenStore();
            //store.SaveFile();
            watch.Stop();
            Console.WriteLine("ok！总花时：{0}", watch.Elapsed);
            Console.Read();

            return;
        }

        private static void InitAppLink() {
            var db = new LinkknilContext();

            for (int i = 4; i <= 21; i++) {
                db.Execute(@"
insert into Lnk_Link values(NEWID(),'24856E5B-691F-410E-B1B6-A11A9FE9C908',
'http://www.jack-liu.com/page/"  
                    +i
                    + @"','//*[@id=""maincol""]/div[2]/h2/a[@href]','xpath',1,GETDATE(),null,null)");
            }

            return;

            var items = new Tuple<string,string>[]{
                new Tuple<string,string>("生活糖果","http://feed.feedsky.com/lifecandy"),
            };

            foreach (var item in items) {
                db.Execute(@"
insert into PF_App values(@appid,'17361dd6-bcb5-4b87-b75e-39bb9a22ab8a','bruce',
@title,'','','搞笑娱乐',null,100,GETDATE(),null,GETDATE(),'bruce',GETDATE(),null)

insert into Lnk_Link values(NEWID(),@appid,
@url,null,'rss',1,GETDATE(),null,null)", 
                new { appid = Guid.NewGuid(), title = item.Item1, url = item.Item2 });
            }
        }

        class UrlPattern {
            public UrlPattern() {

            }

            public UrlPattern(string url, string pattern) {
                this.Url = url;
                this.Pattern = pattern;
                this.CreateTime = DateTime.Now;
            }

            public string Url { get; set; }
            public string Pattern { get; set; }
            public DateTime CreateTime { get; set; }

            public override string ToString() {
                return string.Format("(Url:{0}, Pattern:{1}, CreateTime:{2})", Url, Pattern, CreateTime);
            }
        }

        private static void AliyunSearchServiceTest() {
            SearchServiceTest(new AliyunSearchService());
        }

        private static void SearchCloudSearch() {
            SearchServiceTest(new SearchService());
        }

        private static void SearchServiceTest(ISearchService ss) { 
           ss.Index();

            Console.WriteLine("index success...");
            Console.WriteLine("query bits...");

            var result = ss.Search("bits");

            Console.WriteLine("result:");
            foreach (var item in result) {
                Console.WriteLine("{0}", item.Title);
            }

            Console.WriteLine("query 苹果...");

            result = ss.Search("苹果");

            Console.WriteLine("result:");
            foreach (var item in result) {
                Console.WriteLine("{0}", item.Title);
            }
        }

        private static void RssServiceTest() {
            var url = "http://www.leiphone.com/feed";
            var rss = new RssService();
            var links = rss.PullLink(url);
            foreach (var link in links) {
                Console.WriteLine("{0}: {1}", link.Title, link.Url);
            }
        }

        private static void AliyunFileServiceTest() {
            FileServiceTest(new AliyunFileService());
        }

        private static void AliyunObjectServiceTest() {
            var objectStore = new AliyunFileService();

            Stopwatch watch = Stopwatch.StartNew();
            objectStore.SaveObject("obj1", new UrlPattern("http://foo", "abc中文测试"), "demo");
            watch.Stop();

            Console.WriteLine("保存数据花时：{0}", watch.Elapsed);

            watch.Start();
            var obj = objectStore.GetObject<UrlPattern>("obj1", "demo");
            watch.Stop();

            Console.WriteLine("读取数据花时：{0}", watch.Elapsed);
            Console.WriteLine(obj);
        }

        private static void AliyunMultiObjectServiceTest() {
            var objectStore = new AliyunFileService();
            objectStore.ListObjects<UrlPattern>("demo");
            return;

            int count = 10;
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < count; i++) {
                objectStore.SaveObject("obj" + i, new UrlPattern("http://foo", "abc中文测试" + i), "demo");
            }
            watch.Stop();

            Console.WriteLine("保存数据花时：{0}", watch.Elapsed);

            watch.Start();
            for (int i = 0; i < count; i++) {
                var obj = objectStore.GetObject<UrlPattern>("obj" + i, "demo");
                Console.WriteLine(obj);
            }
            watch.Stop();

            Console.WriteLine("读取数据花时：{0}", watch.Elapsed);
        }

        private static void FileServiceTest(IFileService fileService) {

            var file = File.ReadAllBytes("a.jpg");
            Stream stream = new MemoryStream(file);
            var id = DateTime.Now.ToFileTime().ToString() + ".jpg";

            fileService.Save(id, stream);

            var fileStream = fileService.Get(id);

            var reader = new BinaryReader(fileStream);

            byte[] byData = new byte[1024];
            int i = 0;
            int len;
            var r = new FileStream(id, FileMode.OpenOrCreate);
            while ((len = reader.Read(byData, 0, 1024)) > 0) {
                r.Write(byData, 0, len);
                i += len;
            }

            r.Close();
        }

        private static void ReadabilityTest() {
            var url = "http://www.leiphone.com/304-keats-app-developer.html";
            var nReadabilityTranscoder = new NReadabilityWebTranscoder();
            var transResult = nReadabilityTranscoder.Transcode(new WebTranscodingInput(url));
           // Console.WriteLine(transResult.ExtractedContent);
            File.WriteAllText(string.Format("{0:yyyy-MM-dd HH.mm.ss}.html", DateTime.Now), transResult.ExtractedContent);
        }


        private static PushResult PushToReadability(IEnumerable<string> urls, string mainUrl, ReadabilityService readabilityService) {
            var pushResult = new PushResult();

            foreach (var url in urls) {
                try {
                    var u = url;
                    if (!url.StartsWith("http:")) {
                        u = mainUrl + "/" + url;
                    }

                    var result = readabilityService.Bookmark(u);
                    Console.WriteLine("Success To Read:{0}", u);
                    pushResult.SuccessCount++;
                }
                catch (Exception ex) {
                    Console.WriteLine("Fail To Read:{0},{1}", url, ex.Message);

                    if (ex.Message.Contains("(404)")) {
                        pushResult.FailUrls.Add(url);
                    }
                    else if (ex.Message.Contains("(409)")) {
                        pushResult.DuplicateUrls.Add(url);
                    }

                }
            }

            return pushResult;
        }
    }

    public class PushResult {
        public int SuccessCount { get; set; }
        public int FailCount { get { return FailUrls.Count; } }
        public int DuplicateCount { get { return DuplicateUrls.Count; } }
        public IList<string> FailUrls { get; private set; }
        public IList<string> DuplicateUrls { get; private set; }

        public PushResult() {
            FailUrls = new List<string>();
            DuplicateUrls = new List<string>();
        }
    }
}
