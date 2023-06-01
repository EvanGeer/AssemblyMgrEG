using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyManagerUI;
using AssemblyMgrRevit.Data;
using AssemblyManagerUI.ViewModels;

namespace AssemblyMgrEG.Revit
{
    /// <summary>
    /// Main logic for interacting with Revit:
    /// Builds out views and sheets for new or selected Assembly
    /// </summary>
    /// <remarks>
    /// My typical approach is to have this IExternalCommand serve as an outline
    /// to the application. I try to keep this sparse but informative to help
    /// readability. 
    /// </remarks>
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class AssemblyMgrCmd : ExternalCommandBase
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
            var form = new AssemblyMgrForm(assemblyDataModel);
            form.ShowDialog();
            if (!form.Run)
                return Result.Cancelled;

            // build the views to go on the _sheet
            var viewFactory = new ViewFactory(assemblyDataModel);
            viewFactory.Create3DView();
            viewFactory.Create2DViews();
            viewFactory.CreateBillOfMaterials();

            // build the new _sheet
            var sheet = new AssemblyMgrSheet(viewFactory);
            UiDoc.ActiveView = sheet.Sheet;

            return Result.Succeeded;
        }
    }
}
