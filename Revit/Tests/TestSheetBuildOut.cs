using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrEG.Revit;

namespace AssemblyMgrEG.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class TestSheetBuildOut : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //initialize helper
            var rch = new RevitCommandHelper(commandData);


            var assembly = new AssemblySheetFactory(rch);

            //var formData = new FormData(rch, assembly);
            //assembly.FormData.SelectedTitleBlock = "FabPro_CutSheet_11x17";



            assembly.Create2DView(AssemblyDetailViewOrientation.ElevationTop);
            assembly.Create2DView(AssemblyDetailViewOrientation.ElevationFront);
            assembly.Create3DView();
            assembly.CreateBillOfMaterials();


            var sheet = new AssemblyMgrSheet(rch, assembly.AssemblyDataModel, assembly);

            return Result.Succeeded;
        }
    }

}
