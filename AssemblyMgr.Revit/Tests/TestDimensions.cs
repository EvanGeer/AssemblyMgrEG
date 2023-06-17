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
            // var element = Doc.GetElement(UiDoc.Selection.GetElementIds().FirstOrDefault());
            // var line = (element.Location as LocationCurve).Curve as Line;
            // var geoOptions = new Options
            // {
            //     DetailLevel = ViewDetailLevel.Fine,
            //     IncludeNonVisibleObjects = true,
            //     ComputeReferences = true,
            // };
            // var geometry = element.get_Geometry(geoOptions);
            // var ref1 = line.GetEndPointReference(0);
            // var ref2 = line.GetEndPointReference(1);

            // var allGeo = geometry.OfType<GeometryInstance>().SelectMany(x => x.SymbolGeometry)
            //     .ToList();
            // var allEdges = geometry
            //     .OfType<Solid>()
            //     .SelectMany(x => x.Edges.OfType<Edge>()
            //     .ToList());
            // var edgeLines = allEdges.Select(x => x.AsCurve()).ToList();
            // var edgeRef = allEdges.OrderByDescending(x => x.ApproximateLength).First();
            // var refArray = new ReferenceArray();

            // //var lineRef = allGeo.LastOrDefault(x => x is Line) as Line;
            // refArray.Append(edgeRef.GetEndPointReference(0));
            // refArray.Append(edgeRef.GetEndPointReference(1));

            // var offsetDirection = /*(edgeRef.AsCurve() as Line).Direction.IsAlmostEqualTo(UiDoc.ActiveView.UpDirection)
            //     ? UiDoc.ActiveView.RightDirection.Negate()
            //     : */UiDoc.ActiveView.UpDirection.Negate();
            // var textLine = Line.CreateBound(
            //     offsetDirection + line.GetEndPoint(0),
            //     offsetDirection + line.GetEndPoint(1));


            // var x2xx = geometry
            //     .OfType<Solid>()
            //     .SelectMany(x =>
            //         x.Faces.OfType<GeometryObject>().Union(
            //         x.Edges.OfType<GeometryObject>()))
            //     .ToList();

            // //var rarry2 = geometry.FirstOrDefault(x => x is Face face && face.)

            // var arry1 = x2xx.FirstOrDefault(x => x is Face) as Face;
            // var arry2 = x2xx.Where(x => x is Face).Skip(1).FirstOrDefault() as Face;
            // var arry = new ReferenceArray();
            // arry.Append(arry1.Reference);
            // arry.Append(arry2.Reference);
            // //ConvertToStableRepresentation (Document)	01ab8e50-f929-495c-8fdf-2c4109d5b7fb-0016f4e7:0:LINEAR/1



            // var allStableRefs = allEdges
            //     .Select(x => x.Reference.ConvertToStableRepresentation(Doc))
            //     .ToList();

            //// this line MUST be in the current plane
            //var newLine = Line.CreateUnbound(
            //    line.GetEndPoint(0) + UiDoc.ActiveView.UpDirection,
            //    UiDoc.ActiveView.RightDirection);

            var selectedElements = UiDoc.Selection.GetElementIds()
                .Select(x => Doc.GetElement(x))
                .ToList();
            var distiller = new ElementDistiller(selectedElements);
            var dimensionFactory = new DimensionFactory(Doc, distiller);

            using (var t = new Transaction(Doc, "Test Dims"))
            {
                t.Start();
                dimensionFactory.CreatePipeDimensions(UiDoc.ActiveView);
                //Doc.Create.NewDimension(UiDoc.ActiveView, newLine, refArray);
                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
