using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Test;
using AssemblyMgr.UI.ViewModels;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace AssemblyMgr.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var testSpoolSheetDef = new SpoolSheetDefinition("Sample Spool 101");
            var testController = new AssemblyMgrController();
            var testVM = new AssemblyMgrVM(testSpoolSheetDef, testController)
            {
                // ToDo: get these from the temp folder
                //TitleBlocks = new List<string> { "8.5x11", "11x17", "22x34" },

                
            };


            AssemblyMgrForm mw = new AssemblyMgrForm(testVM);
            mw.ShowDialog();

            //var dir = new DirectoryInfo(@"c:\$\personal\settings\");
            //if (!dir.Exists) dir.Create();
            //var file = new FileInfo(Path.Combine(dir.FullName, "assemblyManager.json"));

            //using (var writer = new StreamWriter(file.FullName))
            //{
            //    var settings = JsonSerializer.Serialize(testSpoolSheetDef);
            //    writer.Write(settings);
            //}
        }
    }
}
