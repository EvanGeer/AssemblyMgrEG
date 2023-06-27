using AssemblyMgr.Revit.DataExtraction;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual)]
    internal class TestGetTitleBlockPng : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var sw = Stopwatch.StartNew();
            var doc = commandData.Application.ActiveUIDocument.Document;
            var titleBlockController = new TitleBlockExtractor(doc);

            using (var tgroup = new TransactionGroup(doc, "export pngs"))
            {
                tgroup.Start();
                foreach (var titleBlock in titleBlockController.TitleBlocksByName)
                {
                    titleBlockController.ExportImage(titleBlock.Value);
                }
                tgroup.RollBack();
            }

            sw.Stop();
            TaskDialog.Show("Done", $"Elapsed: {sw.Elapsed}");
            return Result.Succeeded;
        }
    }
}
