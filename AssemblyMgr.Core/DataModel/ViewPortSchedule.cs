using AssemblyMgr.Core.Geometry;
using System.Collections.ObjectModel;

namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortCustomBom : IViewPort
    {
        public ObservableCollection<BOMFieldDefinition> BOMFields { get; set; }
            = new ObservableCollection<BOMFieldDefinition>();

        public Box2d Outline { get; set; }
        public ViewPortType Type { get; set; }
        public string Title { get; set; }
        public bool IgnoreWelds { get; set; }
        public string ViewTemplate { get; set; }
        public Quadrant DockPoint { get; set; }
    }
    public class ViewPortSchedule : IViewPort
    {
        public Box2d Outline { get; set; }
        public ViewPortType Type { get; set; }
        public string Title { get; set; }
        public bool IgnoreWelds { get; set; }
        public string ViewTemplate { get; set; }
        public Quadrant DockPoint { get; set; }
    }
}
