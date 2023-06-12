using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyMgr.Revit.DataExtraction
{
    internal class ElementDistiller

    {
        public List<FabricationPart> Pipes { get; set; }
        public List<FabricationPart> Joints { get; set; }
        public List<FabricationPart> Fittings { get; set; }

        public List<FabricationPart> Elements { get; set; }

        public ElementDistiller(List<FabricationPart> elements)
        {
            Elements = elements;

            Pipes = elements
                .Where(x => IsPipe(x)).ToList();
            Joints = elements
                .Except(Pipes)
                .Where(x => IsJoint(x)).ToList();
            Fittings = elements
                .Except(Pipes)
                .Except(Joints)
                .ToList();
        }

        public static bool IsPipe(FabricationPart part)
        {
            return part.Location is LocationCurve;
        }

        public static bool IsJoint(FabricationPart part)
        {
            if (IsPipe(part)) return false;

            var connectors = part.ConnectorManager.Connectors
                .OfType<Connector>()
                .ToList();

            double maxLengthForJoint = 2.5 / 12.0;

            bool isJoint = connectors.Count() > 1 &&
                (connectors.FirstOrDefault().Origin
                - connectors.LastOrDefault().Origin).GetLength() < maxLengthForJoint;
            
            return isJoint;
        }

        public static bool IsFitting(FabricationPart part)
            => !IsPipe(part) && !IsFitting(part);
    }
}
