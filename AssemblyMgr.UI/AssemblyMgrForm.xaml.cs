using AssemblyMgr.UI.ViewModels;
using AssemblyMgr.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AssemblyMgr.UI
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class AssemblyMgrForm : Window
    {
        public AssemblyMgrVM AssemblyData { get; set; }
        public bool Run { get; set; } = false;

        public AssemblyMgrForm(AssemblyMgrVM assemblyData)
        {
            DataContext = assemblyData;
            AssemblyData = assemblyData;
            Title = "Assembly Manager: " + AssemblyData?.SpoolSheetDefinition?.AssemblyName;

            InitializeComponent();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var validation = ValidateInputs();
            if (!validation.passed)
            {
                MessageBox.Show(validation.message);
                return;
            }

            Run = true;
            Close();
        }


        private List<(Predicate<SpoolSheetDefinition> isValid, string errorMesssage)> inputValidations
            = new List<(Predicate<SpoolSheetDefinition> isValid, string errorMesssage)>
            {
                (x => !string.IsNullOrEmpty(x.TitleBlock), "A TitelBlock is required."),
                //(x => x.BOMFields.Count > 0, "Please pick at least one column for the Bill of Materials."),
                //(x => x.Scale > 0, "Please select a valid scale.")
            };

        private (bool passed, string message) ValidateInputs()
        {
            var inputErrors = inputValidations
                .Where(x => !x.isValid(AssemblyData.SpoolSheetDefinition))
                .ToList();

            bool validationPassed = inputErrors.Count == 0;
            string message = validationPassed
                ? string.Empty
                : $"One or more validation failed:" +
                    $"\n   - {string.Join("\n   - ", inputErrors.Select(x => x.errorMesssage))}";

            return (validationPassed, message);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
