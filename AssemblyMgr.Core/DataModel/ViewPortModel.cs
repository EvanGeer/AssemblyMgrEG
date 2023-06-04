using AssemblyMgr.Core.Geometry;

namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortModel : IViewPort
    {
        public int Scale { get; set; }

        public bool HasTags { get; set; }
        public bool HasTagLeaders { get; set; }
        public double TagOffset { get; set; }
        public bool HasDimensions { get; set; }

        public Box2d Outline { get; set; }
        public ViewPortType Type { get; set; }
        public string Title { get; set; }
        public Orientation Orientation { get; set; }
        public bool IgnoreWelds { get; set; }
        public string ViewTemplate { get; set; }
    }
}
