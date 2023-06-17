﻿using AssemblyMgr.Core.DataModel;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssemblyMgr.Revit.DataExtraction
{
    public class TitleBlockExtractor
    {
        private Document _doc;
        private static DirectoryInfo _imagesFolder;

        public TitleBlockExtractor(Document document)
        {
            _doc = document;
            _imagesFolder = new DirectoryInfo(Path.Combine(
                Constants.DataFolder.FullName,
                Constants.ImageCacheSubFolder));

            if (!_imagesFolder.Exists) _imagesFolder.Create();
            TitleBlocksByName = getTitleBlockData(_doc);
        }

        public Dictionary<string, FamilySymbol> TitleBlocksByName { get; private set; }

        /// <summary>Gets distinct list of titleblock elements</summary>
        private Dictionary<string, FamilySymbol> getTitleBlockData(Document doc)
        {
            var titleBlocks = new FilteredElementCollector(doc)
               .OfCategory(BuiltInCategory.OST_TitleBlocks)
               .ToElements()
               .OfType<FamilySymbol>()
               .GroupBy(x => x.Name)
               .ToDictionary(
                   x => x.Key,
                   x => x.First());

            return titleBlocks;
        }



        public string ExportImage(FamilySymbol familySymbol)
        {
            using (var t = new Transaction(_doc, "temp create sheets"))
            {
                t.Start();

                var blankSheet = createSheet(familySymbol);

                string sheetName = getImageFileName(familySymbol);
                var options = getImageOptions();
                options.SetViewsAndSheets(new[] { blankSheet.Id });
                _doc.ExportImage(options);

                t.RollBack(); // roll back as not to clutter up user's model
                //t.Commit();

                var imageFile = new FileInfo(Path.Combine(_imagesFolder.FullName, sheetName));
                if (!imageFile.Exists) return null;

                return imageFile.FullName;
            }
        }

        private static string getImageFileName(FamilySymbol familySymbol)
        {
            return $"TitleBlock - Sheet - {familySymbol.Id} - {familySymbol.Name}.png";
        }

        private static ImageExportOptions getImageOptions()
        {
            return new ImageExportOptions
            {
                ZoomType = ZoomFitType.FitToPage,
                PixelSize = Constants.SheetImageWidthPixels,
                FilePath = _imagesFolder.FullName + @"\TitleBlock",
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ShadowViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_300,
                ExportRange = ExportRange.SetOfViews,
            };
        }

        private static ViewSheet createSheet(FamilySymbol titleBlock)
        {
            var sheet = ViewSheet.Create(titleBlock.Document, titleBlock.Id);
            sheet.Name = titleBlock.Name;
            sheet.SheetNumber = titleBlock.Id.IntegerValue.ToString();

            return sheet;
        }

        internal FamilySymbol GetTitleBlock(string titleBlockName)
        {
            if (string.IsNullOrEmpty(titleBlockName)) return null;
            bool titleBlockExists = TitleBlocksByName.TryGetValue(titleBlockName, out FamilySymbol titleBlock);
            if (!titleBlockExists) return null;

            return titleBlock;
        }

        internal string GetImage(string name, bool forceRefresh = false)
        {
            if (string.IsNullOrEmpty(name)) return null;
            bool titleBlockExists = TitleBlocksByName.TryGetValue(name, out FamilySymbol titleBlock);
            if (!titleBlockExists) return null;


            var imageFileName = getImageFileName(titleBlock);
            if (imageFileName == null) return null;

            if (forceRefresh) return ExportImage(titleBlock);

            // else try to find an existing image first
            var imageFilePath = Path.Combine(_imagesFolder.FullName, imageFileName.Trim());
            var imageFile = new FileInfo(imageFilePath);
            if (imageFile.Exists) return imageFile.FullName;

            return ExportImage(titleBlock);
        }
    }
}