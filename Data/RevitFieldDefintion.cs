using AssemblyManagerUI.DataModel;
using Autodesk.Revit.DB;

namespace AssemblyMgrRevit.Data
{
    public class RevitFieldDefintion : BOMFieldDefinition
    {
        public SchedulableField SchedulableField { get; }

        public RevitFieldDefintion(Document doc, SchedulableField schedulableField) : base(schedulableField.GetName(doc))
        {
            SchedulableField = schedulableField;
        }
    }

}
