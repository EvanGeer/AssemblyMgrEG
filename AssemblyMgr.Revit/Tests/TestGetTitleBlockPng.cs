using AssemblyMgr.Revit.Core;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AssemblyMgr.Revit.Tests
{
    [Transaction(TransactionMode.Manual)]
    internal class TestGetTitleBlockPng : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var sw = Stopwatch.StartNew();
            var doc = commandData.Application.ActiveUIDocument.Document;
            var titleBlockController = new TitleBlockController(doc);


            //List<(string Name, ViewSheet BlankSheet)> blankSheets;
            using (var tgroup = new TransactionGroup(doc, "export pngs"))
            {
                tgroup.Start();
                foreach (var titleBlock in titleBlockController.TitleBlocksByName)
                {
                    titleBlockController.ExportImage(titleBlock.Value);
                }
                //using (var t = new Transaction(_doc, "temp create sheets"))
                //{
                //    t.Start();

                //    Func<FamilySymbol, ViewSheet> createSheet = (titleBlock) =>
                //    {
                //        var sheet = ViewSheet.ExportImage(_doc, titleBlock.Id);
                //        sheet.Name = titleBlock.Name;
                //        sheet.SheetNumber = titleBlock.Id.IntegerValue.ToString();

                //        return sheet;
                //    };
                //    blankSheets = titleBlockController.Select(x => (x.Name, createSheet(x)))
                //        .ToList();


                //    var options = new ImageExportOptions
                //    {
                //        ZoomType = ZoomFitType.FitToPage,
                //        PixelSize = 1024,
                //        FilePath = @"c:\$\personal\images\TitleBlock",
                //        FitDirection = FitDirectionType.Horizontal,
                //        HLRandWFViewsFileType = ImageFileType.PNG,
                //        ShadowViewsFileType = ImageFileType.PNG,
                //        ImageResolution = ImageResolution.DPI_72,
                //        ExportRange = ExportRange.SetOfViews,
                //    };
                //    options.SetViewsAndSheets(blankSheets.Select(x => x.BlankSheet.Id).ToList());
                //    _doc.ExportImage(options);

                //    //blankSheets.ForEach(x =>
                //    //{
                //    //    options.FilePath = $@"c:\$\personal\images\x";
                //    //    options.SetViewsAndSheets(new[] { x.BlankSheet.Id });
                //    //    _doc.ExportImage(options);
                //    //});

                //    t.RollBack();
                //}

                tgroup.RollBack();
            }

            sw.Stop();
            TaskDialog.Show("Done", $"Elapsed: {sw.Elapsed}");
            return Result.Succeeded;
        }
    }
}
