using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Revit.DataExtraction;
using AssemblyMgr.Revit.Core;
using AssemblyMgr.Revit.Creation;
using Settings = AssemblyMgr.Core.Serialization.Settings;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Linq;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    internal class TestTagAll : ExternalCommandBase
    {
        public override Result Execute()
        {
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

            using (var t = new Transaction(Doc, $"Build Views"))
            {
                t.Start();

                var viewFactory = new ViewFactory(assemblyInstance, revitAdapter);
                //viewFactory.TagAllPipeElements(UiDoc.ActiveView, false);

                t.Commit();
            }



            return Result.Succeeded;
        }
    }
}
