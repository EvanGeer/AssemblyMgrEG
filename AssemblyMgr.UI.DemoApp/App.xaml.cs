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
            var testVM = new AssemblyMgrVM(testSpoolSheetDef, testController);


            AssemblyMgrForm mw = new AssemblyMgrForm(testVM);
            mw.ShowDialog();



            testSpoolSheetDef.Serialize(file.FullName);

        }
    }
}
