using AssemblyMgr.UI.ViewModels;
using System.ComponentModel;
using System.Windows.Controls;

namespace AssemblyMgr.UI.Components
{
    /// <summary>
    /// Interaction logic for DirectionCtrl_Compass.xaml
    /// </summary>
    public partial class DirectionCtrl_Compass : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public DirectionCtrl_Compass(DirectionVM viewModel)
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
        //    DependencyProperty.Register("Direction", typeof(Direction3d), typeof(DirectionCtrl_Compass), new PropertyMetadata(Direction3d.None));
    }
}
