using System.Collections.ObjectModel;

namespace AssemblyMgr.Core.DataModel
{
    public class SpoolSheetDefinition //: ISpoolSheetDefinition
    {
        public ObservableCollection<IViewPort> ViewPorts { get; set; }
            = new ObservableCollection<IViewPort>();
        public int TitleBlockId { get; set; }
        public string TitleBlock { get; set; }
        public string AssemblyName { get; set; }

        public SpoolSheetDefinition(string assemblyName)
        {
            AssemblyName = assemblyName;
        }
    }
}
