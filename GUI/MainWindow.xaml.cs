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
        public IFormData AssemblyData { get; set; }
        /// <summary>
        /// Observable collection of the names of available schedulable fields, i.e. those not already in the schedule. 
        /// </summary>
        public ObservableCollection<string> AvailFields { get; set; }
        /// <summary>
        /// Constructor for form
        /// Not the cleanest MVVM pattern, but for the sake of time, this works
        /// </summary>
        /// <param name="AssemblyData">Object containing data to populate controls</param>
        public MainWindow(IFormData AssemblyData)
        {
            //setup bindings and ipmort any data from interface
            this.AssemblyData = AssemblyData;
            this.AssemblyData.Cancelled = true; //forces cancel unless user hits okay.


            AvailFields = new ObservableCollection<string>();
            foreach (var item in this.AssemblyData.AvailableBomFields)
                AvailFields.Add(item.parameterName);

            InitializeComponent();
            this.Title = "Assembly Manager: " + this.AssemblyData.AssemblyName;

            TitleBlockListBox.ItemsSource = this.AssemblyData.TitleBlocks;
            bomDataGrid.ItemsSource = this.AssemblyData.BomFields;
            Available.ItemsSource = AvailFields;

            //initialize values - ToDo: use bindings instead
            Ortho.IsChecked = this.AssemblyData.Ortho;
            Top.IsChecked = this.AssemblyData.TopView;
            Front.IsChecked = this.AssemblyData.FrontView;
            TagLeders.IsChecked = this.AssemblyData.TagLeaders;
        }

        private void TitleBlockListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AssemblyData.SelectedTitleBlock = ((ListBox)sender).SelectedItem as string;
        }



        private void Go_Click(object sender, RoutedEventArgs e)
        {
            if (TitleBlockListBox.SelectedItems.Count != 1)
                MessageBox.Show("Please choose a TitleBlock for sheet creation.");
            else
            {
                AssemblyData.Ortho = Ortho.IsChecked ?? false ;
                AssemblyData.TopView = Top.IsChecked ?? false;
                AssemblyData.FrontView = Front.IsChecked ?? false;
                AssemblyData.TagLeaders = TagLeders.IsChecked ?? false;
                AssemblyData.IgnoreWelds = IgnoreWelds.IsChecked ?? false;
                AssemblyData.Cancelled = false;
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
            AvailFields.Remove((string)Available.SelectedItem);
            AssemblyData.BomFields.Add(new Revit.AssemblyMgrBomField(param, param, 1.5/12));

        }

        private void Rem_Click(object sender, RoutedEventArgs e)
        {
            var param = (Revit.AssemblyMgrBomField)bomDataGrid.SelectedItem;
            AvailFields.Add(param.parameterName);
            AvailFields.OrderBy(x => x);
            AssemblyData.BomFields.Remove(param);
        }
    }
}
