using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

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
    class IExternalCommand_CreateAssembly : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //initialize helper
            var rch = new RevitCommandHelper(commandData);

            //Create Assembly
            var assembly = new AssemblyMgrAssembly(rch);

            if (null == assembly.Instance) 
                return Result.Cancelled;

            //Prepare Assembly Data for GUI Interface
            var form = new GUI.MainWindow(assembly.FormData);
            form.ShowDialog();

            //Cancelled form implies cancelled app
            if (assembly.FormData.Cancelled)
                return Result.Cancelled;

            //Build out views
            if (assembly.FormData.Ortho)
                assembly.Create3DView();

            if (assembly.FormData.TopView)
                assembly.Create2DView(AssemblyDetailViewOrientation.ElevationTop);

            if (assembly.FormData.FrontView)
                assembly.Create2DView(AssemblyDetailViewOrientation.ElevationFront);

            //To-Do add some more optionality in form
            assembly.CreateBillOfMaterials();

            //Create new sheet
            var sheet = new AssemblyMgrSheet(rch, assembly.FormData, assembly);

            return Result.Succeeded;
        }
    }
}
