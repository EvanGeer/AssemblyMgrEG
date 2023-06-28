using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Core;
using AssemblyMgr.Revit.Creation;
using Settings = AssemblyMgr.Core.Settings.Settings;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    internal class TestTagAll : ExternalCommandBase
    {
        public override Result Execute()
        {
            // get the data needed for the form and the command
            var selectedElements = UiDoc.Selection.GetElementIds()
                    .Select(x => Doc.GetElement(x))
                    .ToList();

            var tagExtractor = new TagTypeExtractor(Doc);
            var viewDef = new ViewPortDefinition_Model
            {
                HasTags = true,
                FittingTag = tagExtractor.TagTypes.FirstOrDefault(),
                PipeTag = tagExtractor.TagTypes.FirstOrDefault(),
                ItemsToTag = ItemType.Fitting | ItemType.Pipe,
            };
            var activeView = new BuiltViewPort(viewDef, UiDoc.ActiveView);

            using (var t = new Transaction(Doc, $"Build Views"))
            {
                t.Start();

                var distiller = new ElementDistiller(selectedElements);
                var tagFactoty = new TagFactory(Doc, distiller, tagExtractor);
                tagFactoty.CreateTags(activeView);    

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
