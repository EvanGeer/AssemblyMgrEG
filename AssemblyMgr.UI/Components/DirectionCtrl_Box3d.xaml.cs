using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using AssemblyMgr.UI.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AssemblyMgr.UI.Components
{
    /// <summary>
    /// Interaction logic for DirectionCtrl_ViewCube.xaml
    /// </summary>
    public partial class DirectionCtrl_ViewCube : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DirectionCtrl_ViewCube(DirectionVM viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }



        //public Direction3d Direction
        //{
        //    get { return (Direction3d)GetValue(DirectionProperty); }
        //    set { SetValue(DirectionProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty DirectionProperty =
        //    DependencyProperty.Register("Direction", 
        //        typeof(Direction3d), 
        //        typeof(DirectionCtrl_ViewCube), 
        //        new PropertyMetadata(Direction3d.None));

    }
}
