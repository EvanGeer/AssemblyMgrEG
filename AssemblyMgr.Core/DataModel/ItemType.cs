using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyMgr.Core.DataModel
{
    [Flags]
    public enum ItemType
    {
        None = 0,
        Pipe = 1,
        Joint = 2,
        Fitting = 4,
    }
}
