using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Aliyun.OpenServices.OpenStorageService;
using System.IO;
using CoolCode;

namespace Linkknil.StreamStore {
    public class AliyunFileService : IFileService, IObjectStore {
        private OssClient service =
            new OssClient(ConfigurationManager.AppSettings["Aliyun_ApiKey"], ConfigurationManager.AppSettings["Aliyun_ApiSecret"]);

        public void Save(string id, System.IO.Stream stream) {
            var result = service.PutObject("cc-images", "app/" + id, stream, new ObjectMetadata());// { ContentType = "image/jpeg" }

            Console.WriteLine("ETAG:", result.ETag);
        }

        public System.IO.Stream Get(string id) {
            var result = service.GetObject("cc-images", "app/" + id);

            return result.Content;
        }



        public void SaveContent(string id, string text) {
            using (var stream =  text.ToStream()) {
                var result = service.PutObject("cc-contents", "html/" + id, stream, new ObjectMetadata { ContentType = "text/html" });

                Console.WriteLine("ETAG:", result.ETag);
            }
        }

        public string GetContent(string id) {
            var result = service.GetObject("cc-contents", "html/" + id);
            string text;
            using (var reader = new StreamReader(result.Content)) {
                text = reader.ReadToEnd();
            }
            return text;
        }

        public void SaveObject(string key, object obj, string group) { 
            using (var stream = obj.ToStream()) {
                service.PutObject("cc-objects", group + "/" + key, stream, new ObjectMetadata { ContentType = "text/plain" });
            }
        }

        public T GetObject<T>(string key, string group) {
            var ossObject = service.GetObject("cc-objects", group + "/" + key);
            var result = ossObject.Content.ToObject<T>();

            return result;
        }

        public T ListObjects<T>(string group) {
            var request = new ListObjectsRequest("cc-objects");
            var ossObjects =  service.ListObjects(request);
            foreach (var ossObjectSummary in ossObjects.ObjectSummaries) {
                Console.WriteLine(ossObjectSummary.Key);
            }

            return default(T);
        }
    }
}
