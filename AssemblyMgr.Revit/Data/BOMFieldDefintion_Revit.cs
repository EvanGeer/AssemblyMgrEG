using AssemblyMgrShared.DataModel;
using Autodesk.Revit.DB;

namespace AssemblyMgrRevit.Data
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
