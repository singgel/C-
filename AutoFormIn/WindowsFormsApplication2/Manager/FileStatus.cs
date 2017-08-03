using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Manager
{
    public enum FileStatus
    {
        None = 0x0,
        New = 0x1,
        Uploaded = 0x2,
        DeleteFlag = 0x3,
        Deleted = 0x4,
        Update = 0x5
    }
}
