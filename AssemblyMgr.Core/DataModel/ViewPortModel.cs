using AssemblyMgr.Core.Geometry;
using System;

namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortModel : ViewPortDefinition
    {
        public int Scale { get; set; }

        public bool HasTags { get; set; }
        public bool HasTagLeaders { get; set; }
        public double TagOffset { get; set; }
        public bool HasDimensions { get; set; }

        public ElevationOrientation Orientation { get; set; }
    }
}
