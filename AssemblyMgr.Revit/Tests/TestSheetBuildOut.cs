using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.UI.ViewModels;
using AssemblyMgr.Revit.Core;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class TestSheetBuildOut : ExternalCommandBase
    {
        public override Result Execute()
        {
            // build the Assembly from the user selection
            var assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
            if (null == assemblyInstance)
                return Result.Cancelled;

            // get input from the user on how to build the Assembly _sheet
            //var spoolSheetDefinition = new SpoolSheetDefinition(assemblyInstance?.Name);
            //var assemblyDataModel = new AssemblyMgrDataModel(spoolSheetDefinition, assemblyInstance);

            //var assembly = new ViewFactory(assemblyDataModel);
            //assembly.Create2DViews();
            //assembly.Create3DView();
            //assembly.CreateBillOfMaterials();

            //var sheet = new AssemblyMgrSheet(assembly);

            return Result.Succeeded;
        }
    }

}
