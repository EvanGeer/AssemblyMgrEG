using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyMgrEG.GUI
{
    /// <summary>
    /// Interface to pass data to form object
    /// </summary>
    public interface IFormData
    {
        List<string> TitleBlocks { get; set; }
        string SelectedTitleBlock { get; set; }
        
        Revit.BomFieldCollection BomFields {get; set;}
        Revit.BomFieldCollection AvailableBomFields { get; set; }

        bool Cancelled { get; set; }
        bool FrontView { get; set; }
        bool TopView { get; set; }
        bool Ortho { get; set; }
        bool TagLeaders { get; set; }
        bool IgnoreWelds { get; set; }
        int Scale { get; set; }

        string AssemblyName { get; }

    }
}
