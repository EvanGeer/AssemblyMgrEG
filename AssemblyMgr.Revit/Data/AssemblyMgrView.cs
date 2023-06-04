using AssemblyMgr.Core.DataModel;
using Autodesk.Revit.DB;

namespace AssemblyMgr.Revit.Data
{
    public class AssemblyMgrView
    {
        public IViewPort Definition { get; set; }
        public View View { get; set; }

        public AssemblyMgrView(IViewPort definition, View view)
        {
            Definition = definition;
            View = view;
        }
    }
}
