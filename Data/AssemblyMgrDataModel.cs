using AssemblyManagerUI.DataModel;
using AssemblyMgrShared.DataModel;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgrRevit.Data
{
    public class AssemblyMgrDataModel : AssemblyMgrVM
    {
        public Document Doc { get; }
        public RevitAssemblyScheduleDataModel BomDefintion { get; }
        public List<Element> TitleBlockElements { get; private set; }
        public List<SchedulableField> SelectedBomFields =>
            SpoolSheetDefinition.BOMFields
            .OfType<RevitFieldDefintion>()
            .Select(x => x.SchedulableField)
            .ToList();

        public AssemblyInstance Assembly { get; }
        public ElementId SelectedTitleBlockId => TitleBlockElements
            .FirstOrDefault(x => x.Name == SpoolSheetDefinition.TitleBlock)?.Id;

        public AssemblyMgrDataModel(ISpoolSheetDefinition spoolSheetDefinition, AssemblyInstance assembly) : base(spoolSheetDefinition)
        {
            Assembly = assembly;
            Doc = assembly.Document;

            BomDefintion = new RevitAssemblyScheduleDataModel(Doc, Assembly);
            ModelBOMFields = BomDefintion.ModelBOMFields;
            SpoolSheetDefinition.BOMFields = BomDefintion.DefaultFieldList;

            initializeTitelBlockData();
        }

        private void initializeTitelBlockData()
        {
            TitleBlockElements = getTitleBlockData();
            TitleBlocks = TitleBlockElements.Select(x => x.Name).ToList();
        }


        /// <summary>Gets distinct list of titleblock elements</summary>
        private List<Element> getTitleBlockData()
            => new FilteredElementCollector(Doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .ToElements()
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .ToList();



    }
}
