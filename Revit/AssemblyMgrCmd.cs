using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyManagerUI.DataModel;
using AssemblyMgrRevit.Data;
using AssemblyMgrRevit.Revit;

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
    class AssemblyMgrCmd : IExternalCommand, IDevCommand
    {
        public string ProductPrefix { get; }
        public string Product { get; }
        public string SubGroup { get; }
        public string Name { get; } = "Assembly Manager";
        public string ModelName { get; }
        public string ModelYear { get; }

        public Result ExecuteDev(UIApplication uiApp)
        {
            var rch = new RevitCommandHelper(uiApp);
            var assembly = new AssemblySheetFactory(rch);

            if (null == assembly.AssemblyInstance) 
                return Result.Cancelled;

            //Prepare Assembly Data for GUI Interface
            var form = new AssemblyManagerUI.AssemblyMgrForm(assembly.AssemblyDataModel);
            form.ShowDialog();

            //Cancelled form implies cancelled app
            //if (assembly.FormData.Cancelled)
            //    return Result.Cancelled;

            //Build out views
            if (assembly.AssemblyDataModel.SpoolSheetDefinition.PlaceOrthoView)
                assembly.Create3DView();

            if (assembly.AssemblyDataModel.SpoolSheetDefinition.PlaceTopView)
                assembly.Create2DView(AssemblyDetailViewOrientation.ElevationTop);

            if (assembly.AssemblyDataModel.SpoolSheetDefinition.PlaceFrontView)
                assembly.Create2DView(AssemblyDetailViewOrientation.ElevationFront);

            //To-Do add some more optionality in form
            assembly.CreateBillOfMaterials();

            //Create new sheet
            var sheet = new AssemblyMgrSheet(rch, assembly.AssemblyDataModel, assembly);
            uiApp.ActiveUIDocument.ActiveView = sheet.Sheet;

            return Result.Succeeded;            
        }

        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
            => ExecuteDev(commandData.Application);

    }
}
