using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Revit.Data;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AssemblyMgr.Revit.Core
{
    public class AssemblyMangerRevitAdapter : IAssemblyMgrController
    {
        private Document _doc;

        public ScheduleData BomDefintion { get; private set; }
        public AssemblyInstance Assembly { get; }
        public TitleBlockController TitleBlockController { get; }
        public ViewTemplateController ViewTemplateController { get; }

        public AssemblyMangerRevitAdapter(AssemblyInstance assembly)
        {
            _doc = assembly.Document;
            Assembly = assembly;
            TitleBlockController = new TitleBlockController(_doc);
            ViewTemplateController = new ViewTemplateController(_doc);
            BomDefintion = new ScheduleData(_doc, assembly);
        }

        public List<BOMFieldDefinition> BOMFields
            => BomDefintion.ModelBOMFields;

        public FamilySymbol GetTitleBlock(string name)
            => TitleBlockController.GetTitleBlock(name);

        public string GetTitleBlockImage(string name)
            => TitleBlockController.GetImage(name);

        public string RefreshImage(string titleBlock)
            => TitleBlockController.GetImage(titleBlock, forceRefresh: true);

        public List<string> TitleBlocks
            => TitleBlockController.TitleBlocksByName.Keys.ToList();

        public ViewSchedule GetScheduleTemplate(string name)
            => ViewTemplateController.GetScheduleTemplate(name);
        public View GetViewTemplate(string name)
            => ViewTemplateController.GetModelViewTemplate(name);


        public List<string> ViewTemplates 
            => ViewTemplateController.ModelViewTemplates;
        public List<string> ScheduleTemplates
            => ViewTemplateController.ScheduleTemplates;
    }
}
