using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssemblyMgr.UI.Components
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
            _viewModel = this.DataContext as RectangleVM;
        }

        private RectangleVM _viewModel;



        private void DeleteCard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnDeleted is null) return;
            OnDeleted((sender as FrameworkElement)?.DataContext as RectangleVM);
        }

        public Action<RectangleVM> OnDeleted
        {
            get { return (Action<RectangleVM>)GetValue(OnDeletedProperty); }
            set { SetValue(OnDeletedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnDeleted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnDeletedProperty =
            DependencyProperty.Register("OnDeleted", typeof(Action<RectangleVM>), typeof(ViewPortCard), new PropertyMetadata(null));

        private void GoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Type = ViewPortType.None;
        }
    }
}
