using AssemblyMgr.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AssemblyMgr.UI.Components
{
    /// <summary>
    /// Interaction logic for SheduleViewPort.xaml
    /// </summary>
    public partial class SheduleViewPort : UserControl
    {
        private RectangleVM _viewModel;
        private ViewPortVM_CustomBOM _bom => _viewModel?.ViewPort as ViewPortVM_CustomBOM;
        public SheduleViewPort()
        {
            InitializeComponent();
            this.DataContextChanged += setViewModel;
        }

        private void setViewModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as RectangleVM;
        }


        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_bom is null) return;
            _bom.Definition.BOMFields.Add(_bom?.CurrnetAvailableBOMField);
        }

        private void Rem_Click(object sender, RoutedEventArgs e)
        {
            if (_bom is null) return;
            _bom.Definition.BOMFields.Remove(_bom?.CurrnetSelectedBOMField);
        }


    }
}
