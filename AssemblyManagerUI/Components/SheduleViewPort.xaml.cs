using AssemblyManagerUI.ViewModels;
using System;
using System.Collections.Generic;
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

namespace AssemblyManagerUI.Components
{
    /// <summary>
    /// Interaction logic for SheduleViewPort.xaml
    /// </summary>
    public partial class SheduleViewPort : UserControl
    {
        private ViewPortVM _viewModel;
        private ViewPortVM_BOM _bom => _viewModel.ViewPortProps as ViewPortVM_BOM;
        public SheduleViewPort()
        {
            InitializeComponent();
            this.DataContextChanged += setViewModel;
        }

        private void setViewModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as ViewPortVM;
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is null) return;
            _viewModel.SpoolSheetDefinition.BOMFields.Add(_bom?.CurrnetAvailableBOMField);
        }

        private void Rem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel is null) return;
            _viewModel.SpoolSheetDefinition.BOMFields.Remove(_bom?.CurrnetSelectedBOMField);
        }


    }
}
