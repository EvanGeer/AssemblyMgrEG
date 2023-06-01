using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrEG.Revit.Core;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestSchedulePlacement : ExternalCommandBase
    {
        public override Result Execute()
        {
            var sel = UiDoc.Selection.GetElementIds().First();
            var sched = (ScheduleSheetInstance)Doc.GetElement(sel);

            using (Transaction t = new Transaction(Doc, "Test Schedule Placement"))
            {
                t.Start();

                var len = sched.get_BoundingBox(UiDoc.ActiveView).Max.X - sched.get_BoundingBox(UiDoc.ActiveView).Min.X;
                sched.Point = new XYZ(17.0 / 12.0 - len, 11.0 / 12.0, 0);

                t.Commit();
            }
            return Result.Succeeded;
        }
    }

}
