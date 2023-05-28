using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyMgrRevit.Revit.Tests
{
    [Transaction(TransactionMode.Manual)]
    internal class TestGetTitleBlockPng : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var sw = Stopwatch.StartNew();
            var doc = commandData.Application.ActiveUIDocument.Document;
            var tBlocks = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                //.OfClass(typeof(FamilyInstance))
                //.ToElementIds()
                .ToElements()
                .OfType<FamilySymbol>()
                .ToList()
                ;

            //TaskDialog.Show("Sheets", "")

            List<(string Name, ViewSheet BlankSheet)> blankSheets;
            using (var tgroup = new TransactionGroup(doc, "export pngs"))
            {
                tgroup.Start();
                using (var t = new Transaction(doc, "temp create sheets"))
                {
                    t.Start();

                    Func<FamilySymbol, ViewSheet> createSheet = (titleBlock) =>
                    {
                        var sheet = ViewSheet.Create(doc, titleBlock.Id);
                        sheet.Name = titleBlock.Name;
                        sheet.SheetNumber = titleBlock.Id.IntegerValue.ToString();

                        return sheet;
                    };
                    blankSheets = tBlocks.Select(x => (x.Name, createSheet(x)))
                        .ToList();


                var options = new ImageExportOptions
                {
                    ZoomType = ZoomFitType.FitToPage,
                    PixelSize = 1024,
                    FilePath = @"c:\$\personal\images\TitleBlock",
                    FitDirection = FitDirectionType.Horizontal,
                    HLRandWFViewsFileType = ImageFileType.PNG,
                    ShadowViewsFileType = ImageFileType.PNG,
                    ImageResolution = ImageResolution.DPI_72,
                    ExportRange = ExportRange.SetOfViews,
                };
                options.SetViewsAndSheets(blankSheets.Select(x => x.BlankSheet.Id).ToList());
                doc.ExportImage(options);

                    //blankSheets.ForEach(x =>
                    //{
                    //    options.FilePath = $@"c:\$\personal\images\x";
                    //    options.SetViewsAndSheets(new[] { x.BlankSheet.Id });
                    //    doc.ExportImage(options);
                    //});

                    t.RollBack();
                }

                tgroup.RollBack();
            }

            sw.Stop();
            TaskDialog.Show("Done", $"Elapsed: {sw.Elapsed}");
            return Result.Succeeded;
        }
    }
}
