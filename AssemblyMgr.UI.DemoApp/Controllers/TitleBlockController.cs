//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace AssemblyMgr.UI.Test
//{
//    internal class TitleBlockController : ITitleBlockController
//    {
//        private Dictionary<string, FileInfo> _titleBlockImages
//            = new Dictionary<string, FileInfo>();

//        private const string _defaultPath = @"C:\$\Personal\images\TitleBlock - Sheet - 2562563 - 11x17 Titleblock.png";

//        public string GetTitleBlockImage(string name)
//        {
//            if (string.IsNullOrEmpty(name) || !_titleBlockImages.ContainsKey(name)) return _defaultPath;
            
//            return _titleBlockImages[name]?.FullName ?? _defaultPath;
//        }

//        public List<string> TitleBlocks()
//        {
//            // ToDo: Change this path to a sample data folder in repo
//            var titleBlockImageFolder = @"C:\$\Personal\images";
//            var files = new DirectoryInfo(titleBlockImageFolder)
//                    .GetFiles()
//                    .Where(x => x.Extension.Equals(".png",StringComparison.InvariantCultureIgnoreCase));

//            _titleBlockImages = files
//                .Select(x => new
//                {
//                    Name = x.Name.Replace(x.Extension, "").Split('-').Last().Trim(),
//                    ImageFile = x,
//                })
//                .ToDictionary(
//                    x => x.Name,
//                    x =>  x.ImageFile
//                );

//            return _titleBlockImages.Keys.ToList();
//        }

//    }
//}
