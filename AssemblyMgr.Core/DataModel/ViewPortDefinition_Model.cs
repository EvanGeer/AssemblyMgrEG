using AssemblyMgr.Core.Geometry;
using System;

namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortDefinition_Model : ViewPortDefinition
    {
        public int Scale { get; set; }

        public string JointTag { get; set; }
        public string PipeTag { get; set; }
        public string FittingTag { get; set; }
        public bool HasTags { get; set; }
        public bool HasDimensions { get; set; }
        public ItemsToTag ItemsToTag { get; set; }

        //public bool HasTagLeaders { get; set; }
        //public double TagOffset { get; set; }
        //public ElevationOrientation Orientation { get; set; }
    }
}
