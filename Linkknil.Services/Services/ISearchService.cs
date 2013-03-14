using Linkknil.Entities;
using System;
using System.Collections.Generic;

namespace Linkknil.Services {
   public interface ISearchService {
        void Index();
        IEnumerable<ContentViewModel> Search(string keyword, int page = 0, int pageSize = 10);
    }
}
