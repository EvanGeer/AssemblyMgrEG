using AssemblyMgrShared.DataModel;
using System.Collections.ObjectModel;

namespace AssemblyManagerUI.ViewModels
{
    public class SpoolSheetDefinition : ISpoolSheetDefinition
    {
        public ObservableCollection<BOMFieldDefinition> BOMFields { get; set; }
        = new ObservableCollection<BOMFieldDefinition>();
        public string TitleBlock { get; set; }

        public bool TagLeaders { get; set; }
        public bool PlaceOrthoView { get; set; } = true;
        public bool PlaceFrontView { get; set; } = true;
        public bool PlaceTopView { get; set; } = true;

        public bool IgnoreWelds { get; set; }
        public int Scale { get; set; } = 48;
        public string AssemblyName { get; set; }

        public SpoolSheetDefinition() { }
        public SpoolSheetDefinition(string assemblyName)
        {
            AssemblyName = assemblyName;
        }
    }
}
