using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgr.Revit.Data;
using AssemblyMgr.UI;
using AssemblyMgr.UI.ViewModels;
using AssemblyMgr.Core.DataModel;

namespace AssemblyMgr.Revit.Core
{
    /// <summary>
    /// Main logic for interacting with Revit.Core:
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
            // build the Assembly from the user selection
            var assemblyInstance = AssemblyInstanceFactory.CreateBySelection(UiDoc);
            if (null == assemblyInstance)
                return Result.Cancelled;

            // get input from the user on how to build the Assembly sheet
            var spoolSheetDefinition = new SpoolSheetDefinition(assemblyInstance?.Name);
            var revitAdapter = new AssemblyMangerRevitAdapter(assemblyInstance);
            var viewModel = new AssemblyMgrVM(spoolSheetDefinition, revitAdapter);
            //var assemblyDataModel = new AssemblyMgrDataModel(spoolSheetDefinition, assemblyInstance);
            var form = new AssemblyMgrForm(viewModel);
            form.ShowDialog();
            if (!form.Run)
                return Result.Cancelled;

            // build the views to go on the sheet
            var viewFactory = new ViewFactory(assemblyInstance, revitAdapter);
            var views = viewFactory.CreateViews(spoolSheetDefinition.ViewPorts);

            //var viewFactory = new ViewFactory(assemblyInstance, spoolSheetDefinition, revitAdapter.BomDefintion);
            //viewFactory.Create3DView();
            //viewFactory.Create2DViews();
            //viewFactory.CreateBillOfMaterials();
            using (var t = new Transaction(Doc, $"Test Place 3 view"))
            {
                t.Start();


                // build the new sheet
                var sheetFactory = new SheetFactory(views);
                var titleBlockId = revitAdapter.GetTitleBlock(spoolSheetDefinition.TitleBlock)?.Id;
                var sheet = sheetFactory.Create(assemblyInstance, titleBlockId);
                t.Commit();
                UiDoc.ActiveView = sheet;
            }


            return Result.Succeeded;
        }
    }
}
