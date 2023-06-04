using AssemblyMgr.Core.DataModel;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Data
{
    public class BOMFieldDefintion_Revit : BOMFieldDefinition
    {
        public SchedulableField SchedulableField { get; }

        public BOMFieldDefintion_Revit(Document doc, SchedulableField schedulableField) : base(schedulableField.GetName(doc))
        {
            SchedulableField = schedulableField;
        }
    }

}
