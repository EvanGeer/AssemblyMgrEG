using AssemblyMgr.Revit.DataExtraction;
using Autodesk.Revit.DB;

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
                try
                {
                    var line = (straight.Location as LocationCurve).Curve as Line;

                    // make sure we can place the dimension in the current view
                    if (line.Direction.IsAlmostEqualTo(view.ViewDirection)
                        || line.Direction.IsAlmostEqualTo(view.ViewDirection.Negate()))
                        continue;

                    var refArray = new ReferenceArray();

                    var lineRefs = Distiller.GetCenterLineRefArray(straight);
                    if (lineRefs?.Count != 2) return;

                    lineRefs.ForEach(x => refArray.Append(x));

                    var offsetDirection = line.Direction.CrossProduct(view.ViewDirection).Negate();

                    var textLine = Line.CreateUnbound(
                        offsetDirection + line.GetEndPoint(0),
                        offsetDirection.CrossProduct(view.ViewDirection));

                    view.Document.Create.NewDimension(view, textLine, refArray);
                }
                catch 
                {
                    // ToDo: Add logging
                    // log and continue... but we don't have logging yet
                }
            }
        }

    }
}
