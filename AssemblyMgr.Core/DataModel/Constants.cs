using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace AssemblyMgr.Core.DataModel
{
    public static class Constants
    {
        public const int SheetImageWidthPixels = 1024;
        public const string AppName = "AssemblyManager";
        public static DirectoryInfo DataFolder = InitializeStorageFolder();
        public const string ImageCacheSubFolder = @"TitleBlockImageCache";

        private static DirectoryInfo InitializeStorageFolder()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var storagaPath = Path.Combine(appDataFolder, AppName);
            var storageDirectory = new DirectoryInfo(storagaPath);

            if (!storageDirectory.Exists) storageDirectory.Create();

            return storageDirectory;
        }

    }
}
