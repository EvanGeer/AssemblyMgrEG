using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using AssemblyMgrEG.Revit;

namespace AssemblyMgrEG.Revit.Tests
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]

    class TestBOMGeneration : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //initialize helper
            var rch = new RevitCommandHelper(commandData);
            var doc = rch.ActiveDoc;

            var assembly = new AssemblyMgrAssembly(rch);
            assembly.CreateBillOfMaterials();

            return Result.Succeeded;
        }
    }
}
