using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AssemblyMgr.Core.Geometry
{
    public class Box3d
    {
        public Box3d(Vector3 max, Vector3 min)
        {
            Max = max;
            Min = min;
        }

        public Vector3 Max { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Center => (Min + Max) / 2.0f;
    }
}
