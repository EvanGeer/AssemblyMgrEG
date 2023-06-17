﻿using AssemblyMgr.Core.DataModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AssemblyMgr.UI.DemoApp.Mocks
{
    public class AssemblyMgrController : IAssemblyMgrController
    {
        private static Dictionary<string, FileInfo> _titleBlockImages
            = new Dictionary<string, FileInfo>();

        private const string _defaultPath = @"C:\$\Personal\images\TitleBlock - Sheet - 2562563 - 11x17 Titleblock.png";

        public string GetTitleBlockImage(string name)
        {
            if (string.IsNullOrEmpty(name) || !_titleBlockImages.ContainsKey(name)) return _defaultPath;

            return _titleBlockImages[name]?.FullName ?? _defaultPath;
        }

        public List<string> TitleBlocks { get; } = getSampleTitleBlocks();
        public List<string> ViewTemplates { get; } = new List<string>
        {
            "2D Template",
            "Plane Template",
            "Elevation Template",
            "3D Template",
        };
        public List<string> ScheduleTemplates { get; } = new List<string>
        {
            "BOM Template",
            "Schedule Template",
        };
        public List<string> TagTypes { get; } = new List<string>
        {
            "Pipe Tag 1",
            "Pipe Tag 2",
            "Joint Tag",
            "BOM Tag",
        };

        private static List<string> getSampleTitleBlocks()
        {
            // ToDo: Change this path to a sample data folder in repo
            var titleBlockImageFolder = @"C:\$\Personal\images";
            var files = new DirectoryInfo(titleBlockImageFolder)
                    .GetFiles()
                    .Where(x => x.Extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase));

            _titleBlockImages = files
                .Select(x => new
                {
                    Name = x.Name.Replace(x.Extension, "").Split('-').Last().Trim(),
                    ImageFile = x,
                })
                .ToDictionary(
                    x => x.Name,
                    x => x.ImageFile
                );

            return _titleBlockImages.Keys.ToList();
        }

        public string RefreshImage(string titleBlock)
        {
            throw new NotImplementedException();
        }
    }
}