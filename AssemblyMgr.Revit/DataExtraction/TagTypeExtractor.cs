using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.DataExtraction
{
    public class TagTypeExtractor
    {
        private Document _doc;
        private Dictionary<string, FamilySymbol> _tagTypesByName;
        public List<string> TagTypes { get; }

        public TagTypeExtractor(Document document)
        {
            _doc = document;
            _tagTypesByName = getTagTypes();
            TagTypes = _tagTypesByName.Keys.ToList();
        }


        /// <summary>Gets distinct list of titleblock elements</summary>
        private Dictionary<string, FamilySymbol> getTagTypes()
        {
            var tagCategories = new LogicalOrFilter(new List<ElementFilter>
            {
                new ElementCategoryFilter(BuiltInCategory.OST_Tags),
                new ElementCategoryFilter(BuiltInCategory.OST_PipeTags),
                new ElementCategoryFilter(BuiltInCategory.OST_PipeFittingTags),
                new ElementCategoryFilter(BuiltInCategory.OST_PipeAccessoryTags),
                new ElementCategoryFilter(BuiltInCategory.OST_PipeInsulationsTags),
                new ElementCategoryFilter(BuiltInCategory.OST_CableTrayTags),
                new ElementCategoryFilter(BuiltInCategory.OST_ConduitTags),
                new ElementCategoryFilter(BuiltInCategory.OST_FabricationPipeworkTags),
                new ElementCategoryFilter(BuiltInCategory.OST_GenericModelTags),
                new ElementCategoryFilter(BuiltInCategory.OST_MultiCategoryTags),
            });

            var templates = new FilteredElementCollector(_doc)
                .WherePasses(tagCategories)
               //.OfCategory(BuiltInCategory.OST_FabricationPipeworkTags)
               //.OfClass(typeof(FamilySymbol))
               .ToElements()
               .OfType<FamilySymbol>()
               .GroupBy(x => x.Name)
               .ToDictionary(
                   x => x.Key,
                   x => x.First());

            return templates;
        }


        public FamilySymbol GetTagType(string name) => GetTemplate(name);
        private FamilySymbol GetTemplate(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            bool templateExists = _tagTypesByName.TryGetValue(name, out FamilySymbol template);
            if (!templateExists) return null;

            return template;
        }
    }
}
