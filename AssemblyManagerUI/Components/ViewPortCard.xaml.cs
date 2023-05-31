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
    /// Interaction logic for ViewPortCard.xaml
    /// </summary>
    public partial class ViewPortCard : UserControl
    {
        public ViewPortCard()
        {
            InitializeComponent();
            this.DataContextChanged += setViewModel;
        }

        private void setViewModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as ViewPortVM;
        }

        private ViewPortVM _viewModel;



        private void DeleteCard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnDeleted is null) return;
            OnDeleted((sender as FrameworkElement)?.DataContext as ViewPortVM);
        }

        public Action<ViewPortVM> OnDeleted
        {
            get { return (Action<ViewPortVM>)GetValue(OnDeletedProperty); }
            set { SetValue(OnDeletedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnDeleted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnDeletedProperty =
            DependencyProperty.Register("OnDeleted", typeof(Action<ViewPortVM>), typeof(ViewPortCard), new PropertyMetadata(null));

        private void GoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Type = ViewPortType.None;
        }
    }
}
