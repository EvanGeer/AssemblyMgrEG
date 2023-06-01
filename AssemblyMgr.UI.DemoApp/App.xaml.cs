using AssemblyManagerUI.Test;
using AssemblyManagerUI.ViewModels;
using System.Windows;

namespace AssemblyManagerUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var testSpoolSheetDef = new SpoolSheetDefinition();
            var testController = new AssemblyMgrController();
            var testVM = new AssemblyMgrVM(testSpoolSheetDef, testController)
            {
                // ToDo: get these from the temp folder
                //TitleBlocks = new List<string> { "8.5x11", "11x17", "22x34" },

                
            };

            AssemblyMgrForm mw = new AssemblyMgrForm(testVM);
            mw.Show();
        }
    }
}
