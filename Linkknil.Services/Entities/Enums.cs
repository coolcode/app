using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Linkknil.Entities {
    public enum AppStatus {
        Draft = 0,
        Pending = 1,
        Publish = 100,
        Offline = -2,
        Reject = -1,
    }

    public enum ItemStatus{
        Disabled = 0,
        Enabled = 1
    }

    public enum LinkStatus {
        Disabled = 0,
        Enabled = 1,
        Diging = 50
    }

    public enum PullStatus
    {
        None = 0,
        Fail = -1,
        Success = 1,
        Duplicate = 2
    }
}
