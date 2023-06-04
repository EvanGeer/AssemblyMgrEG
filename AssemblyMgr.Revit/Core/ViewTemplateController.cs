using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.Core
{
    public class ViewTemplateController
    {
        private Document _doc;
        private Dictionary<string, View> _modelViewTemplatesByName;
        private Dictionary<string, ViewSchedule> _scheduleTemplatesByName;
        public List<string> ModelViewTemplates { get; }
        public List<string> ScheduleTemplates { get; }

        public ViewTemplateController(Document document)
        {
            _doc = document;
            _modelViewTemplatesByName = getTemplates<View>(BuiltInCategory.OST_Views);
            _scheduleTemplatesByName = getTemplates<ViewSchedule>(BuiltInCategory.OST_Schedules);

            ModelViewTemplates = _modelViewTemplatesByName.Keys.ToList();
            ScheduleTemplates = _scheduleTemplatesByName.Keys.ToList();
        }


        /// <summary>Gets distinct list of titleblock elements</summary>
        private Dictionary<string, T> getTemplates<T>(BuiltInCategory category)
            where T : View
        {
            var templates = new FilteredElementCollector(_doc)
               .OfCategory(category)
               .ToElements()
               .OfType<T>()
               .Where(x => x.IsTemplate)
               .GroupBy(x => x.Name)
               .ToDictionary(
                   x => x.Key,
                   x => x.First());            
            
            return templates;
        }


        public View GetModelViewTemplate(string name) => GetTemplate(name, _modelViewTemplatesByName);
        public ViewSchedule GetScheduleTemplate(string name) => GetTemplate(name, _scheduleTemplatesByName);
        private T GetTemplate<T>(string name, Dictionary<string, T> lookup)
            where T : View
        {
            if (string.IsNullOrEmpty(name)) return null;
            bool templateExists = lookup.TryGetValue(name, out T template);
            if (!templateExists) return null;

            return template;
        }
    }
}
