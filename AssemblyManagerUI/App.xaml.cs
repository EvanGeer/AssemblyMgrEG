using AssemblyManagerUI.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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

            var testSpoolSheetDef = new TestData.SpoolSheetDefinition();
            var testVM = new AssemblyMgrVM(testSpoolSheetDef)
            {
                TitleBlocks = new List<string> { "8.5x11", "11x17", "22x34" },
                ModelBOMFields = new List<BOMFieldDefinition>
                {
                    new BOMFieldDefinition("Tag"),
                    new BOMFieldDefinition("Description"),
                    new BOMFieldDefinition("Length"),
                    new BOMFieldDefinition("End Prep 1"),
                    new BOMFieldDefinition("End Prep 2"),
                }
            };

            AssemblyMgrForm mw = new AssemblyMgrForm(testVM);
            mw.Show();
        }
    }
}
