using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace AssemblyMgrEG.Revit
{
    /// <summary>
    /// Main data object for passing information between the form and Revit
    /// </summary>
    public class FormData : AssemblyMgrEG.GUI.IFormData
    {
        private RevitCommandHelper rch { get; set; }
        private FilteredElementCollector titleBlockFEC { get; set; }
        public List<string> TitleBlocks { get; set; }
        public string SelectedTitleBlock { get; set; }
        public ElementId SelectedTitleBlockId { get => getTitbleBlockId(); }
        public BomFieldCollection BomFields { get; set; }
        public BomFieldCollection AvailableBomFields { get; set; }

        public bool Cancelled { get; set; } = false;

        public bool FrontView { get; set; } = true;
        public bool TopView { get; set; } = true;

        public bool Ortho { get; set; } = true;

        public bool TagLeaders { get; set; } = false;

        public bool IgnoreWelds { get; set; } = true;

        public int Scale { get; set;  }

        public string AssemblyName { get; set; }

        public FormData(RevitCommandHelper Helper)
        {
            rch = Helper;
            TitleBlocks = getTitleBlockData();
        }

        private ElementId getTitbleBlockId()
        {
            return titleBlockFEC.FirstOrDefault(x => x.Name == SelectedTitleBlock).Id;
        }        private List<string> getTitleBlockData()
        {
            titleBlockFEC = new FilteredElementCollector(rch.ActiveDoc);
            titleBlockFEC.OfCategory(BuiltInCategory.OST_TitleBlocks);

            return titleBlockFEC.Select(x => x.Name).Distinct().ToList();
        }

    }
}
