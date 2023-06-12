using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.Revit.Core;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestDimensions : ExternalCommandBase
    {
        public override Result Execute()
        {
            var element = Doc.GetElement(UiDoc.Selection.GetElementIds().FirstOrDefault());
            var line = (element.Location as LocationCurve).Curve as Line;
            var geoOptions = new Options
            {
                DetailLevel = ViewDetailLevel.Fine,
                IncludeNonVisibleObjects = true,
                ComputeReferences = true,
            };
            var geometry = element.get_Geometry(geoOptions);
            var ref1 = line.GetEndPointReference(0);
            var ref2 = line.GetEndPointReference(1);
            
            var allGeo = geometry.OfType<GeometryInstance>().SelectMany(x => x.SymbolGeometry).ToList();
            var refArray = new ReferenceArray();

            var lineRef = allGeo.FirstOrDefault(x => x is Line) as Line;
            refArray.Append(lineRef.GetEndPointReference(0));
            refArray.Append(lineRef.GetEndPointReference(1));

            var offsetDirection = lineRef.Direction.IsAlmostEqualTo(UiDoc.ActiveView.UpDirection)
                ? UiDoc.ActiveView.RightDirection.Negate()
                : UiDoc.ActiveView.UpDirection.Negate();
            var textLine = Line.CreateBound(
                offsetDirection + line.GetEndPoint(0),
                offsetDirection + line.GetEndPoint(1));

            using (var t = new Transaction(Doc, "Test Dims"))
            {
                t.Start();
                Doc.Create.NewDimension(UiDoc.ActiveView, textLine, refArray);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
