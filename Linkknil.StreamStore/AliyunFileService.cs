using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Aliyun.OpenServices.OpenStorageService;

namespace Linkknil.StreamStore {
    public class AliyunFileService : IFileService {
        private OssClient service =
            new OssClient(ConfigurationManager.AppSettings["Aliyun_ApiKey"], ConfigurationManager.AppSettings["Aliyun_ApiSecret"]);

        public void Save(string id, System.IO.Stream stream) {
            var result = service.PutObject("cc-images", "small/" +id, stream, new ObjectMetadata());// { ContentType = "image/jpeg" }
             
            Console.WriteLine("ETAG:",result.ETag);
        }

        public System.IO.Stream Get(string id) {
            var result = service.GetObject("cc-images", "small/" + id);

            return result.Content;
        }
    }
}
