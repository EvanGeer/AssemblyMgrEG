using AssemblyMgr.Core.Geometry;

namespace AssemblyMgr.Core.DataModel
{
    public interface IViewPort
    {
        //int Scale { get; }
        //bool HasTags { get; }
        //bool HasTagLeaders { get; }
        //bool HasDimensions { get; }
        Box2d Outline { get; }
        ViewPortType Type { get; }
    }
}
