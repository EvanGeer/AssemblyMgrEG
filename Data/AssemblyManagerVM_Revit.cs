using AssemblyManagerUI.DataModel;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyMgrRevit.Data
{
    public class AssemblyManagerDataModel : AssemblyMgrVM
    {
        public Document Doc { get; }
        public List<Element> TitleBlockElements { get; }
        public ElementId SelectedTitleBlockId => TitleBlockElements
            .FirstOrDefault(x => x.Name == SpoolSheetDefinition.TitleBlock)?.Id;

        public AssemblyManagerDataModel(ISpoolSheetDefinition spoolSheetDefinition, Document doc) : base(spoolSheetDefinition)
        {
            Doc = doc;
            SpoolSheetDefinition.BOMFields = _defaultBomFieldList;
            TitleBlockElements = getTitleBlockData();
            TitleBlocks = TitleBlockElements.Select(x => x.Name).ToList();
        }

        private ObservableCollection<BOMFieldDefinition> _defaultBomFieldList 
            = new ObservableCollection<BOMFieldDefinition>()
            {
                ("Mark", "Tag", 0.5/12.0),
                ("Count","Qty",0.5/12.0),
                ("Size","Size",1/12.0),
                ("Product Short Description","Description",3/12.0),
                ("Part Material","Material",3/12.0),
                ("Length","Length",1.5/12.0)
            };

        /// <summary>Gets distinct list of titleblock elements</summary>
        private List<Element> getTitleBlockData()
            => new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .ToList();
    }
}
