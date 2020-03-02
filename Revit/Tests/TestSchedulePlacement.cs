using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrEG.Revit;

namespace AssemblyMgrEG.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class TestSchedulePlacement : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //initialize helper
            var rch = new RevitCommandHelper(commandData);

            var sel = rch.UiDoc.Selection.GetElementIds().First();
            var sched = (ScheduleSheetInstance)rch.ActiveDoc.GetElement(sel);

            using (Transaction t = new Transaction(rch.ActiveDoc, "Test Schedule Placement"))
            {
                t.Start();

                //sched.Location.Move(new XYZ(1, -.5, 0));
                var len = sched.get_BoundingBox(rch.UiDoc.ActiveView).Max.X - sched.get_BoundingBox(rch.UiDoc.ActiveView).Min.X;
                sched.Point = new XYZ(17.0 / 12.0 - len, 11.0/ 12.0, 0);

                t.Commit();
            }
            return Result.Succeeded;
        }
    }

}
