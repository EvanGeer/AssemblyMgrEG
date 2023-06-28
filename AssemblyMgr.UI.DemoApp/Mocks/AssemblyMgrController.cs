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

        public string GetTitleBlockImage(string name)
        {
            if (string.IsNullOrEmpty(name) || !_titleBlockImages.ContainsKey(name)) return null;

            return _titleBlockImages[name]?.FullName;
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
            var titleBlockImageFolder = App.StorageLocation;
            var files = getFiles(titleBlockImageFolder);

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

        private static IEnumerable<FileInfo> getFiles(DirectoryInfo titleBlockImageFolder)
        {
            var imageDirectory = new DirectoryInfo(titleBlockImageFolder.FullName);

            var files = getImageFilesFromFolder(imageDirectory);

            if (files.Count > 0) return files;

            Properties.Resources.E1_30x42_Horizontal.Save(Path.Combine(imageDirectory.FullName, "E1 30x42 Horizontal.png"));
            Properties.Resources.B_11_x_17_Horizontal.Save(Path.Combine(imageDirectory.FullName, "B 11x17 Horizontal.png"));
            Properties.Resources.A0_metric.Save(Path.Combine(imageDirectory.FullName, "A0 Metric.png"));

            return getImageFilesFromFolder(imageDirectory);
        }

        private static List<FileInfo> getImageFilesFromFolder(DirectoryInfo imageDirectory)
        {
            return imageDirectory
                    .GetFiles()
                    .Where(x => x.Extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
        }

        public string RefreshImage(string titleBlock)
        {
            throw new NotImplementedException();
        }
    }
}
