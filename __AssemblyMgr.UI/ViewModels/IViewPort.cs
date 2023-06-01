using AssemblyMgrShared.DataModel;

namespace AssemblyManagerUI.ViewModels
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
