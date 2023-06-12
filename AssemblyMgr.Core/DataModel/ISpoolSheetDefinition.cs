using System.Collections.ObjectModel;

namespace AssemblyMgr.Core.DataModel
{

    public interface ISpoolSheetDefinition
    {
        string TitleBlock { get; set; }

        bool PlaceOrthoView { get; set; }
        bool PlaceFrontView { get; set; }
        bool PlaceTopView { get; set; }

        int Scale { get; set; }
        string AssemblyName { get; }
    }
}
