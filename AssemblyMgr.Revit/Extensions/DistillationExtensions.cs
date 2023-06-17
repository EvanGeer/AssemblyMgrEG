using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyMgr.Revit.Extensions
{
    internal static class DistillationExtensions
    {
        public static List<Connector> GetConnectors(this Element element)
        {
            var connectorSet =
                element is FabricationPart part
                ? part.ConnectorManager.Connectors
                : element is FamilyInstance family
                ? family.MEPModel.ConnectorManager.Connectors
                : null;

            if (connectorSet is null) return null;

            var connectors = connectorSet.OfType<Connector>().ToList();

            return connectors;
        }

        public static bool IsPipe(this Element element)
        {
            return element.Location is LocationCurve;
        }

        public static bool IsJoint(this Element element)
        {
            if (IsPipe(element)) return false;


            var connectors = GetConnectors(element);
            if (connectors is null) return false;

            double maxLengthForJoint = 2.5 / 12.0;

            bool isJoint = connectors.Count() > 1 &&
                (connectors.FirstOrDefault().Origin
                - connectors.LastOrDefault().Origin).GetLength() < maxLengthForJoint;

            return isJoint;
        }

        public static bool IsFitting(this Element element)
            => !IsPipe(element) && !IsFitting(element);

        public static XYZ GetDirection(this Element element)
        {
            if (element.Location is LocationCurve location 
                && location.Curve is Line line)
                return line.Direction;

            var connectors = element
                .GetConnectors()
                ?.Where(x => x.ConnectorType == ConnectorType.End)
                ?.ToList();
            if (!(connectors?.Count > 0)) return null;

            var elementDirection = connectors
                .Select(x => x.CoordinateSystem.BasisZ)
                .Aggregate((x, y) => x + y)
                .Negate();
            return elementDirection;
        }

        public static Line GetCenterLine(this Element element)
        {
            return (element.Location as LocationCurve)?.Curve as Line;
        }
    }
}
