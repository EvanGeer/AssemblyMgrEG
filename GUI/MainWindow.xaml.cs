using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssemblyMgrEG.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// To-Do: Clean up MVVM architecture - works but could be cleaner
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Form interface
        /// </summary>
        private IFormData assemblyData;
        /// <summary>
        /// Observable collection of the names of available schedulable fields, i.e. those not already in the schedule. 
        /// </summary>
        private ObservableCollection<string> availFields;
        /// <summary>
        /// Constructor for form
        /// Not the cleanest MVVM pattern, but for the sake of time, this works
        /// </summary>
        /// <param name="AssemblyData">Object containing data to populate controls</param>
        public MainWindow(IFormData AssemblyData)
        {
            //setup bindings and ipmort any data from interface
            assemblyData = AssemblyData;
            assemblyData.Cancelled = true; //forces cancel unless user hits okay.


            availFields = new ObservableCollection<string>();
            foreach (var item in assemblyData.AvailableBomFields)
                availFields.Add(item.parameterName);

            InitializeComponent();
            this.Title = "Assembly Manager: " + assemblyData.AssemblyName;

            TitleBlockListBox.ItemsSource = assemblyData.TitleBlocks;
            bomDataGrid.ItemsSource = assemblyData.BomFields;
            Available.ItemsSource = availFields;

            //initialize values - ToDo: use bindings instead
            Ortho.IsChecked = assemblyData.Ortho;
            Top.IsChecked = assemblyData.TopView;
            Front.IsChecked = assemblyData.FrontView;
            TagLeders.IsChecked = assemblyData.TagLeaders;
        }

        private void TitleBlockListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            assemblyData.SelectedTitleBlock = ((ListBox)sender).SelectedItem as string;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (TitleBlockListBox.SelectedItems.Count != 1)
                MessageBox.Show("Please choose a TitleBlock for sheet creation.");
            else
            {
                assemblyData.Ortho = Ortho.IsChecked ?? false ;
                assemblyData.TopView = Top.IsChecked ?? false;
                assemblyData.FrontView = Front.IsChecked ?? false;
                assemblyData.TagLeaders = TagLeders.IsChecked ?? false;
                assemblyData.IgnoreWelds = IgnoreWelds.IsChecked ?? false;
                assemblyData.Cancelled = false;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string param = (string)Available.SelectedItem;
            availFields.Remove((string)Available.SelectedItem);
            assemblyData.BomFields.Add(new Revit.AssemblyMgrBomField(param, param, 1.5/12));

        }

        private void Rem_Click(object sender, RoutedEventArgs e)
        {
            var param = (Revit.AssemblyMgrBomField)bomDataGrid.SelectedItem;
            availFields.Add(param.parameterName);
            availFields.OrderBy(x => x);
            assemblyData.BomFields.Remove(param);
        }
    }
}
