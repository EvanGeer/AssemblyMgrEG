using System;
using System.Collections.Generic;
using System.Text;

namespace AssemblyMgr.Core.DataModel
{
    [Flags]
    public enum ItemsToTag
    {
        None = 0,
        Pipes = 1,
        Joints = 2,
        Fittings = 4,
    }
}
