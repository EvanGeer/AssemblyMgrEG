using AssemblyManagerUI.DataModel;
using System.Collections.ObjectModel;

namespace AssemblyManagerUI.TestData
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
    }
}
