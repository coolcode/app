using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Linkknil.StreamStore {
    public interface IFileService
    {
        void Save(string id, Stream stream);
        Stream Get(string id);
    }
}
