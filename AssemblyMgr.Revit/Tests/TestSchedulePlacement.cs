using AssemblyMgr.Revit.Core;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Creation;
using Settings = AssemblyMgr.Core.Settings.Settings;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestSchedulePlacement : ExternalCommandBase
    {
        public override Result Execute()
        {
            var sheet = UiDoc.ActiveView as ViewSheet;
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
                .Where(x => x is ViewPortDefinition_Schedule).ToList();

            spoolSheetDefinition.ViewPorts
                = new ObservableCollection<ViewPortDefinition>(scheduleDefs);

            var revitAdapter = new AssemblyMangerRevitAdapter(assemblyInstance);

            List<BuiltViewPort> views;


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
