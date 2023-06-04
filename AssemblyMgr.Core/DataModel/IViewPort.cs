using AssemblyMgr.Core.Geometry;

namespace AssemblyMgr.Core.DataModel
{
   public interface IViewPort
    {
        Box2d Outline { get; set; }
        ViewPortType Type { get; set; }
        string Title { get; set; }
        bool IgnoreWelds { get; set; }
        string ViewTemplate { get; set; }
    }
}
