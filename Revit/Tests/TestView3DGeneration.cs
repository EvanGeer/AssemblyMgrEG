using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrEG.Revit;
using AssemblyMgrRevit.Data;
using AssemblyManagerUI.DataModel;

namespace AssemblyMgrEG.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestView3DGeneration : ExternalCommandBase
    {
        public override Result Execute()
        {
            // build the assembly from the user selection
            var assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
            if (null == assemblyInstance)
                return Result.Cancelled;

            // get input from the user on how to build the assembly sheet
            var spoolSheetDefinition = new SpoolSheetDefinition(assemblyInstance?.Name);
            var assemblyDataModel = new AssemblyMgrDataModel(spoolSheetDefinition, assemblyInstance);

            var assembly = new ViewFactory(assemblyDataModel);
            assembly.Create3DView();

            return Result.Succeeded;
        }
    }
}
