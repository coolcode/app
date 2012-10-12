using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace Linkknil.StreamStore {
    public class NavenStore {

        public void SaveFile() {

            var documentStore = new DocumentStore {
                ConnectionStringName = "CloudBird"
            };
            documentStore.Initialize();
           /*
           var session = documentStore.OpenSession();
           session.Store(new { Url = "coolcode@live.com", Name = "Bruce Lee", China = "布鲁斯李" }, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

          session.SaveChanges();
        */
        //Stream data = new MemoryStream(new byte[] { 1, 2, 3 }); // don't forget to load the data from a file or something!
        var file = File.ReadAllBytes("a.jpg");
        Stream data = new MemoryStream(file);
        documentStore.DatabaseCommands.PutAttachment("image/b", null, data,
                                                     new RavenJObject { { "FileName", "a.jpg" } ,{ "Description", "2012-12-12 Image Test " } });
       
            var attachment = documentStore.DatabaseCommands.GetAttachment("image/a");
            Console.WriteLine(attachment.Metadata["FileName"]);

            var s = attachment.Data();

            var reader = new BinaryReader(s);

            byte[] byData = new byte[1024];
            int i = 0;
            int len;
            var r = new FileStream("4.jpg", FileMode.OpenOrCreate);
            while ((len = reader.Read(byData,0, 1024))>0) {
                r.Write(byData, 0, len);
                i+=len;
            }

            r.Close();
            
            //Console.WriteLine();
        }
    }
}
