using AssemblyMgr.Revit.DataExtraction;
using Autodesk.Revit.DB;
using System.Numerics;

namespace AssemblyMgr.Revit.Creation
{
    internal class DimensionFactory
    {
        public Document Doc { get; set; }
        public ElementDistiller Distiller { get; set; }

        public DimensionFactory(Document doc, ElementDistiller distiller)
        {
            Doc = doc;
            Distiller = distiller;
        }


        public void CreatePipeDimensions(View view)
        {
            foreach (var straight in Distiller.Pipes)
            {
                var line = (straight.Location as LocationCurve).Curve as Line;

                // make sure we can place the dimension in the current view
                if (line.Direction.IsAlmostEqualTo(view.ViewDirection)
                    || line.Direction.IsAlmostEqualTo(view.ViewDirection.Negate()))
                    continue;

                var refArray = new ReferenceArray();

                //var lineRefs = allGeo.FirstOrDefault(x => x is Line) as Line;
                var lineRefs = Distiller.GetCenterLineRefArray(straight);
                if (lineRefs?.Count != 2) return;

                lineRefs.ForEach(x => refArray.Append(x));

                var offsetDirection = line.Direction.CrossProduct(view.ViewDirection).Negate();
                /*line.Direction.IsAlmostEqualTo(view.UpDirection)
                || line.Direction.IsAlmostEqualTo(view.UpDirection.Negate())
                    ? view.RightDirection//.Negate()
                    : view.UpDirection;//.Negate();*/

                //// this line MUST be in the current plane
                //var newLine = Line.CreateUnbound(
                //    line.GetEndPoint(0) + UiDoc.ActiveView.UpDirection,
                //    UiDoc.ActiveView.RightDirection);

                var textLine = Line.CreateUnbound(
                    offsetDirection + line.GetEndPoint(0),
                    offsetDirection.CrossProduct(view.ViewDirection));

                try
                {
                    view.Document.Create.NewDimension(view, textLine, refArray);
                }
                catch { }
            }
        }

    }
}
