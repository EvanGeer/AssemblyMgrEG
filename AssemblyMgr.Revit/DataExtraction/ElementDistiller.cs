using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Revit.Extensions;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.DataExtraction
{
    internal class ElementDistiller
    {
        public List<Element> Pipes => _typeMap[ItemType.Pipe];
        public List<Element> Joints => _typeMap[ItemType.Joint];
        public List<Element> Fittings => _typeMap[ItemType.Fitting];

        public List<Element> Elements { get; set; }

        public ElementDistiller(List<Element> elements)
        {
            Elements = elements;

            var pipes = elements
                .Where(x => x.IsPipe())
                .ToList();
            _typeMap.Add(ItemType.Pipe, pipes);

            var joints = elements
                .Except(Pipes)
                .Where(x => x.IsJoint())
                .ToList();
            _typeMap.Add(ItemType.Joint, joints);

            var fittings = elements
                .Except(Pipes)
                .Except(Joints)
                .ToList();
            _typeMap.Add(ItemType.Fitting, fittings);
        }

        public List<Element> this[ItemType type]
        {
            get
            {
                if (!_typeMap.ContainsKey(type))
                    return new List<Element>();

                return _typeMap[type];
            }
        }

        private Dictionary<ElementId, List<Reference>> _centerlineRefs { get; set; }
            = new Dictionary<ElementId, List<Reference>>();


        private Reference getEndPointRef(GeometryObject geometryObject, int index)
        {
            if (geometryObject is Curve curve)
                return curve.GetEndPointReference(index);

            if (geometryObject is Edge edge)
                return edge.GetEndPointReference(index);

            return null;
        }

        public List<Reference> GetCenterLineRefArray(Element element)
        {
            if (!_centerlineRefs.ContainsKey(element.Id))
            {
                var geometry = getGeometry(element);
                var refs = Enumerable.Range(0, 2)
                    .Select(x => getEndPointRef(geometry, x))
                    .ToList();

                _centerlineRefs[element.Id] = refs;
            }

            return _centerlineRefs[element.Id];
        }


        private bool isCenterline(GeometryObject geometryElement, Element element)
        {
            if (!((geometryElement as Line ?? (geometryElement as Edge)?.AsCurve()) is Line geometryLine)) return false;

            if (!(element.GetCenterLine() is Line centerLine)) return false;

            return
                (geometryLine.GetEndPoint(0).IsAlmostEqualTo(centerLine.GetEndPoint(0), 0.1)
                 || geometryLine.GetEndPoint(0).IsAlmostEqualTo(centerLine.GetEndPoint(1), 0.1))
                &&
                (geometryLine.GetEndPoint(1).IsAlmostEqualTo(centerLine.GetEndPoint(0), 0.1)
                 || geometryLine.GetEndPoint(1).IsAlmostEqualTo(centerLine.GetEndPoint(1), 0.1));
        }

        private GeometryObject getGeometry(Element element)
        {
            var symbolGeometry = element.get_Geometry(_geometryOptions)
                .OfType<GeometryInstance>()
                .SelectMany(x => x.SymbolGeometry)
                .ToList();

            if (symbolGeometry.FirstOrDefault(x => isCenterline(x, element)) is Line symbolGeometryCenterline)
                return symbolGeometryCenterline;
            if (symbolGeometry.FirstOrDefault(x => x is Line) is Line cline)
                return cline;

            var allEdges = element.get_Geometry(_geometryOptions)
                .OfType<Solid>()
                .SelectMany(x => x.Edges.OfType<Edge>()
                .ToList());
            var lkj = allEdges.FirstOrDefault(x => x.AsCurve() is Line && x.Reference != null).AsCurve();
            var edgeCenterLine = allEdges.FirstOrDefault(x => isCenterline(x.AsCurve(), element))?.AsCurve() as Line;
            var edgeCenterLine2 = allEdges.FirstOrDefault(x => isCenterline(x.AsCurve(), element));
            return edgeCenterLine2;
        }

        private Options _geometryOptions = new Options
        {
            DetailLevel = ViewDetailLevel.Fine,
            IncludeNonVisibleObjects = true,
            ComputeReferences = true,
        };

        private Dictionary<ItemType, List<Element>> _typeMap { get; set; }
        = new Dictionary<ItemType, List<Element>>();

    }
}
