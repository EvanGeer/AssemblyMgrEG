using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Serialization;
using AssemblyMgr.UI.DemoApp.Mocks;
using AssemblyMgr.UI.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace AssemblyMgr.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static DirectoryInfo StorageLocation
            = new DirectoryInfo(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                $@"AssemblyMgr.UI.Demo\titleBlockImageCache\"));

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!StorageLocation.Exists) StorageLocation.Create();
            var file = new FileInfo(Path.Combine(StorageLocation.FullName, "assemblyManager.xml"));

            var testSpoolSheetDef = Settings.DeSerialize<SpoolSheetDefinition>(file)
                ?? new SpoolSheetDefinition();
            var testController = new AssemblyMgrController();
            var testVM = new AssemblyMgrVM(testSpoolSheetDef, testController)
            {
                // ToDo: get these from the temp folder
                //TitleBlocks = new List<string> { "8.5x11", "11x17", "22x34" },


            };


            AssemblyMgrForm mw = new AssemblyMgrForm(testVM);
            mw.ShowDialog();



            testSpoolSheetDef.Serialize(file.FullName);

            //using (var writer = new StreamWriter(file.FullName))
            //{
            //    var settings = JsonSerializer.Serialize(testSpoolSheetDef);
            //    writer.Write(settings);
            //    var forecast = JsonSerializer.Deserialize<SpoolSheetDefinition>(settings);
            //}
        }


    }
}
