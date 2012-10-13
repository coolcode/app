using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Linkknil.Services;
using Linkknil.StreamStore;
using NReadability;

namespace Linkknil.ConsoleKits {
    class Program {
        class UrlPattern {
            public UrlPattern() {

            }

            public UrlPattern(string url, string pattern) {
                this.Url = url;
                this.Pattern = pattern;
            }

            public string Url { get; set; }
            public string Pattern { get; set; }
        }

        static void Main(string[] args)
        {
            RssServiceTest();
            //AliyunFileServiceTest();
            //var store = new NavenStore();
            //store.SaveFile();
            Console.WriteLine("ok");
            Console.Read();

            return;
            Console.Read();
        }

        private static void RssServiceTest()
        {
            var url = "http://www.leiphone.com/feed";
            var rss = new RssService();
            var links = rss.PullLink(url);
            foreach (var link in links)
            {
                Console.WriteLine("{0}: {1}",link.Title, link.Url);
            }
        }

        private static void AliyunFileServiceTest() {
            FileServiceTest(new AliyunFileService());
        }

        private static void FileServiceTest(IFileService fileService) {

            var file = File.ReadAllBytes("a.jpg");
            Stream stream = new MemoryStream(file);
            var id = DateTime.Now.ToFileTime().ToString();

            fileService.Save(id, stream);

            var fileStream = fileService.Get(id);

            var reader = new BinaryReader(fileStream);

            byte[] byData = new byte[1024];
            int i = 0;
            int len;
            var r = new FileStream(id + ".jpg", FileMode.OpenOrCreate);
            while ((len = reader.Read(byData, 0, 1024)) > 0) {
                r.Write(byData, 0, len);
                i += len;
            }

            r.Close();
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
