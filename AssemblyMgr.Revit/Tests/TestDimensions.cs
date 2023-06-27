using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.Revit.Core;
using AssemblyMgr.Revit.Creation;
using AssemblyMgr.Revit.DataExtraction;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestDimensions : ExternalCommandBase
    {
        public override Result Execute()
        {
            var selectedElements = UiDoc.Selection.GetElementIds()
                .Select(x => Doc.GetElement(x))
                .ToList();
            var distiller = new ElementDistiller(selectedElements);
            var dimensionFactory = new DimensionFactory(Doc, distiller);

            using (var t = new Transaction(Doc, "Test Dims"))
            {
                t.Start();
                dimensionFactory.CreatePipeDimensions(UiDoc.ActiveView);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
