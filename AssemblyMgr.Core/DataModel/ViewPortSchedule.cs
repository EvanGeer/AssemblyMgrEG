using AssemblyMgr.Core.Geometry;
using System.Collections.ObjectModel;

namespace AssemblyMgr.Core.DataModel
{
    public class ViewPortCustomBom : ViewPortDefinition
    {
        public ObservableCollection<BOMFieldDefinition> BOMFields { get; set; }
            = new ObservableCollection<BOMFieldDefinition>();

        public Quadrant DockPoint { get; set; }
    }
    public class ViewPortSchedule : ViewPortDefinition
    {
        public Quadrant DockPoint { get; set; }
    }
}
