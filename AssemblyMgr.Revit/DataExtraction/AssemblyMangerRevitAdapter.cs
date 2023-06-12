using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.DataExtraction
{
    public class AssemblyMangerRevitAdapter : IAssemblyMgrController
    {
        private Document _doc;

        public AssemblyInstance Assembly { get; }
        public TitleBlockExtractor TitleBlockController { get; }
        public ViewTemplateExtractor ViewTemplateController { get; }
        public TagTypeExtractor TagTypeController { get; }

        public AssemblyMangerRevitAdapter(AssemblyInstance assembly)
        {
            _doc = assembly.Document;
            Assembly = assembly;
            TitleBlockController = new TitleBlockExtractor(_doc);
            ViewTemplateController = new ViewTemplateExtractor(_doc);
            TagTypeController = new TagTypeExtractor(_doc);
        }

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


        public List<string> TagTypes
             => TagTypeController.TagTypes;
  }
}
