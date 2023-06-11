using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.Revit.Core;
using AssemblyMgr.Core.DataModel;
using Settings = AssemblyMgr.Core.Serialization.Settings;
using Autodesk.Revit.UI.Selection;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using AssemblyMgr.Revit.Data;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestSchedulePlacement : ExternalCommandBase
    {
        public override Result Execute()
        {
            //var sel = UiDoc.Selection.GetElementIds().First();
            //var sched = (ScheduleSheetInstance)Doc.GetElement(sel);

            //using (Transaction t = new Transaction(Doc, "Test Schedule Placement"))
            //{
            //    t.Start();

            //    var len = sched.get_BoundingBox(UiDoc.ActiveView).Max.X - sched.get_BoundingBox(UiDoc.ActiveView).Min.X;
            //    sched.Point = new XYZ(17.0 / 12.0 - len, 11.0 / 12.0, 0);

            //    t.Commit();
            //}
            ViewSheet sheet = UiDoc.ActiveView as ViewSheet;
            if (sheet == null)
                return Result.Cancelled;


            // get the data needed for the form and the command
            var assemblyInstance = UiDoc.ActiveView.IsAssemblyView
                ? Doc.GetElement(UiDoc.ActiveView.AssociatedAssemblyInstanceId) 
                    as AssemblyInstance
                : UiDoc.Selection.GetElementIds()
                    .Select(x => Doc.GetElement(x))
                    .OfType<AssemblyInstance>()
                    .FirstOrDefault();

            var assemblyManager = new AssemblyMgrCmd();
            var spoolSheetDefinition = Settings.DeSerialize<SpoolSheetDefinition>(assemblyManager.SettingsFile)
                ?? new SpoolSheetDefinition();

            // get the schedules only
            var scheduleDefs = spoolSheetDefinition.ViewPorts
                .Where(x => x is ViewPortSchedule).ToList();

            spoolSheetDefinition.ViewPorts
                = new ObservableCollection<ViewPortDefinition>(scheduleDefs);

            var revitAdapter = new AssemblyMangerRevitAdapter(assemblyInstance);

            List<AssemblyMgrView> views;


            using (var t = new Transaction(Doc, $"Build Views"))
            {
                t.Start();

                var viewFactory = new ViewFactory(assemblyInstance, revitAdapter);
                views = viewFactory.CreateViews(spoolSheetDefinition.ViewPorts);

                t.Commit();
            }

            using (var t = new Transaction(Doc, $"Place Views on Sheet"))
            {
                t.Start();

                var sheetFactory = new SheetFactory(views);
                var titleBlockId = revitAdapter.GetTitleBlock(spoolSheetDefinition.TitleBlock)?.Id;
                sheetFactory.PlaceViews(sheet);

                t.Commit();
            }

            return Result.Succeeded;
        }
    }

}
