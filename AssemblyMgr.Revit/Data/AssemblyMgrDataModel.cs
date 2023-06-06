//using AssemblyMgr.UI.ViewModels;
//using AssemblyMgr.Core.DataModel;
//using Autodesk.Revit.DB;
//using System.Collections.Generic;
//using System.Linq;

//namespace AssemblyMgr.Revit.Data
//{
//    public class AssemblyMgrDataModel : AssemblyMgrVM
//    {
//        public Document _doc { get; }
//        public ScheduleData BomDefintion { get; }
//        public List<Element> TitleBlockElements { get; private set; }
//        public List<SchedulableField> SelectedBomFields =>
//            ViewPort.ViewPorts
//            .OfType<BOMFieldDefintion_Revit>()
//            .Select(x => x.SchedulableField)
//            .ToList();

//        public AssemblyInstance Assembly { get; }
//        public ElementId SelectedTitleBlockId => TitleBlockElements
//            .FirstOrDefault(x => x.Name == ViewPort.TitleBlock)?.Id;

//        public AssemblyMgrDataModel(ISpoolSheetDefinition spoolSheetDefinition, AssemblyInstance Assembly) 
//            : base(spoolSheetDefinition)
//        {
//            Assembly = Assembly;
//            _doc = Assembly.Document;

//            BomDefintion = new ScheduleData(_doc, Assembly);
//            ModelBOMFields = BomDefintion.ModelBOMFields;
//            ViewPort.ViewPorts = BomDefintion.DefaultFieldList;

//            initializeTitelBlockData();
//        }

//        private void initializeTitelBlockData()
//        {
//            TitleBlockElements = getTemplates();
//            //ViewTemplatesByName = ViewTemplatesByName.Select(x => x.Name).ToList();
//        }


//        /// <summary>Gets distinct list of titleblock elements</summary>
//        private List<Element> getTemplates()
//            => new FilteredElementCollector(_doc)
//                .OfCategory(BuiltInCategory.OST_TitleBlocks)
//                .ToElements()
//                .GroupBy(x => x.Name)
//                .Select(x => x.First())
//                .ToList();



//    }
//}
