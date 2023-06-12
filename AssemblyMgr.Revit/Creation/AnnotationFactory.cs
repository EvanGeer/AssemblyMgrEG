using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Extensions;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.Creation
{
    internal class AnnotationFactory
    {
        public Document Doc { get; set; }
        public ElementDistiller Distiller { get; set; }
        public TagTypeExtractor TagTypeExtractor { get; }

        public AnnotationFactory(Document doc, ElementDistiller distiller, TagTypeExtractor tagTypeExtractor)
        {
            Doc = doc;
            Distiller = distiller;
            TagTypeExtractor = tagTypeExtractor;
        }

        /// <summary>
        /// Tags all elements in a 3D view.
        /// </summary>
        /// <param name="view">View to tag</param>
        /// <param name="tagOffset">Tag off set if using leaders. Will be 0 if leaders are not used.</param>
        public void CreateTags(BuiltViewPort assemblyMgrView)
        {
            if (!(assemblyMgrView.Definition is ViewPortDefinition_Model viewDef)) return;

            // ToDo: custom tag offset per tag type
            var tagOffset = 6 / 12.0;

            //build out tags
            if (assemblyMgrView.View is View3D view3d && !view3d.IsLocked)
                view3d.SaveOrientationAndLock();

            foreach(var tagSet in viewDef.ItemsToTag.GetFlags(x => x != ItemsToTag.None).ToList())
            {
                List<FabricationPart> elementSet;
                string tagName;

                switch(tagSet)
                {
                    case ItemsToTag.Fittings:
                        elementSet = Distiller.Fittings;
                        tagName = viewDef.FittingTag;
                        break;
                    case ItemsToTag.Joints:
                        elementSet = Distiller.Joints;
                        tagName = viewDef.JointTag;
                        break;
                    case ItemsToTag.Pipes:
                        elementSet = Distiller.Pipes;
                        tagName = viewDef.PipeTag;
                        break;
                    default:
                        elementSet = null;
                        tagName = null; 
                        break;
                }

                var tagType = TagTypeExtractor.GetTagType(tagName);

                elementSet.ForEach(x => placeTag(x, tagType, tagOffset, assemblyMgrView.View));
            }
        }

        private void placeTag(FabricationPart elem, FamilySymbol tagType, double tagOffset, View view)
        {
            var elemRef = new Reference(elem);
            var elementCenter = elem.get_BoundingBox(view).GetCenter();
            var connectors = elem.ConnectorManager.Connectors
                .OfType<Connector>()
                .Where(x => x.ConnectorType == ConnectorType.End);

            var elementDirection = connectors
                .Select(x => x.CoordinateSystem.BasisZ)
                .Aggregate((x, y) => x + y);

            // this should be normal to the element
            var offsetDiretion = elementDirection.IsAlmostEqualTo(XYZ.Zero)
                ? connectors.FirstOrDefault()?.CoordinateSystem.BasisZ.CrossProduct(view.ViewDirection)
                : elementDirection.Negate();

            if (offsetDiretion.IsAlmostEqualTo(view.RightDirection, 0.1)
                || offsetDiretion.IsAlmostEqualTo(view.RightDirection.Negate(), 0.1))
                offsetDiretion = view.RightDirection.Negate();

            if (offsetDiretion.IsAlmostEqualTo(view.UpDirection, 0.1)
                || offsetDiretion.IsAlmostEqualTo(view.UpDirection.Negate(), 0.1))
                offsetDiretion = view.UpDirection.Negate();


            var tagXYZ = elementCenter + offsetDiretion * tagOffset;

            var tag = IndependentTag.Create(Doc, tagType.Id, view.Id, elemRef, true/* viewDef.HasTagLeaders*/, TagOrientation.Horizontal, tagXYZ);
            tag.LeaderEndCondition = LeaderEndCondition.Free;
            tag.SetLeaderEnd(elemRef, elementCenter);
            tag.TagHeadPosition = tagXYZ;
        }


        /// <summary>
        /// Out of time on this one ... see a future release 
        /// </summary>
        /// <remarks>
        /// I haven't done a ton with fabrication part annotations, and this one will require
        /// some research on my part. 
        /// </remarks>
        /// <param name="view"></param>
        public void CreatePipeDimensions(View view)
        {
            foreach (var straight in Distiller.Pipes)
            {
                var line = (straight.Location as LocationCurve).Curve as Line;
                if (line.Direction.IsAlmostEqualTo(view.ViewDirection)
                    || line.Direction.IsAlmostEqualTo(view.ViewDirection.Negate())) continue;

                var geoOptions = new Options
                {
                    DetailLevel = ViewDetailLevel.Fine,
                    IncludeNonVisibleObjects = true,
                    ComputeReferences = true,
                };
                var geometry = straight.get_Geometry(geoOptions);
                var allGeo = geometry
                    .OfType<GeometryInstance>()
                    .SelectMany(x => x.SymbolGeometry)
                    .ToList();

                var refArray = new ReferenceArray();

                var lineRef = allGeo.FirstOrDefault(x => x is Line) as Line;
                refArray.Append(lineRef.GetEndPointReference(0));
                refArray.Append(lineRef.GetEndPointReference(1));

                var offsetDirection = line.Direction.IsAlmostEqualTo(view.UpDirection)
                    || line.Direction.IsAlmostEqualTo(view.UpDirection.Negate())
                    ? view.RightDirection//.Negate()
                    : view.UpDirection;//.Negate();

                var textLine = Line.CreateBound(
                    offsetDirection + line.GetEndPoint(0),
                    offsetDirection + line.GetEndPoint(1));

                Doc.Create.NewDimension(view, textLine, refArray);
            }
        }

    }
}
