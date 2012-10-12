using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace Linkknil.StreamStore {
    public class WebFileService : IFileService {
        private string path = @"C:\Products\git\app\Linkknil.Web\Upload";
        public void Save(string id, System.IO.Stream stream) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            using (var file = new FileStream(Path.Combine(path, id), FileMode.OpenOrCreate)) {
                stream.CopyTo(file);
                file.Close();
            }
        }

        public System.IO.Stream Get(string id) {
            return File.OpenRead(Path.Combine(path, id));
        }
    }
}
