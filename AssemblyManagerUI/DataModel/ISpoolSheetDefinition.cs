using System.Collections.ObjectModel;

namespace AssemblyManagerUI.DataModel
{
    public interface ISpoolSheetDefinition
    {
        ObservableCollection<BOMFieldDefinition> BOMFields { get; set; }
        string TitleBlock { get; set; }

        bool TagLeaders { get; set; }
        bool PlaceOrthoView { get; set; }
        bool PlaceFrontView { get; set; }
        bool PlaceTopView { get; set; }

        bool IgnoreWelds { get; set; }
        int Scale { get; set; }
        string AssemblyName { get; }
    }
}
