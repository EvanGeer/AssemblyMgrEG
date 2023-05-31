using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrRevit.Data;
using AssemblyManagerUI.ViewModels;

namespace AssemblyMgrEG.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestSheetBuildOut : ExternalCommandBase
    {
        public override Result Execute()
        {
            // build the assembly from the user selection
            var assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
            if (null == assemblyInstance)
                return Result.Cancelled;

            // get input from the user on how to build the assembly _sheet
            var spoolSheetDefinition = new SpoolSheetDefinition(assemblyInstance?.Name);
            var assemblyDataModel = new AssemblyMgrDataModel(spoolSheetDefinition, assemblyInstance);

            var assembly = new ViewFactory(assemblyDataModel);
            assembly.Create2DViews();
            assembly.Create3DView();
            assembly.CreateBillOfMaterials();

            var sheet = new AssemblyMgrSheet(assembly);

            return Result.Succeeded;
        }
    }

}
