using AssemblyMgr.Core.DataModel;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Creation
{
    public class BuiltViewPort
    {
        public ViewPortDefinition Definition { get; set; }
        public View View { get; set; }

        public BuiltViewPort(ViewPortDefinition definition, View view)
        {
            Definition = definition;
            View = view;
        }
    }
}
