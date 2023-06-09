using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;
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
        public DirectionCtrl_ViewCube()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void notifyCornersChanged()
        {
            this.Notify(PropertyChanged, new[] 
            {
                nameof(TopFrontRight),
                nameof(TopFrontLeft),
                nameof(TopBackRight),
                nameof(TopBackLeft),
                nameof(BottomFrontRight),
                nameof(BottomFrontLeft),
                nameof(BottomBackRight),
                nameof(BottomBackLeft),
            });
        }

        public bool TopFrontRight
        {
            get => Direction == ViewCubeCorner.TopFrontRight;
            set
            {
                if (value == true) Direction = ViewCubeCorner.TopFrontRight;
                notifyCornersChanged();
            }
        }
        public bool TopFrontLeft
        {
            get => Direction == ViewCubeCorner.TopFrontLeft;
            set
            {
                if (value == true) Direction = ViewCubeCorner.TopFrontLeft;
                notifyCornersChanged();
            }
        }
        public bool TopBackRight
        {
            get => Direction == ViewCubeCorner.TopBackRight;
            set
            {
                if (value == true) Direction = ViewCubeCorner.TopBackRight;
                notifyCornersChanged();
            }
        }
        public bool TopBackLeft
        {
            get => Direction == ViewCubeCorner.TopBackLeft;
            set
            {
                if (value == true) Direction = ViewCubeCorner.TopBackLeft;
                notifyCornersChanged();
            }
        }
        public bool BottomFrontRight
        {
            get => Direction == ViewCubeCorner.BottomFrontRight;
            set
            {
                if (value == true) Direction = ViewCubeCorner.BottomFrontRight;
                notifyCornersChanged();
            }
        }
        public bool BottomFrontLeft
        {
            get => Direction == ViewCubeCorner.BottomFrontLeft;
            set
            {
                if (value == true) Direction = ViewCubeCorner.BottomFrontLeft;
                notifyCornersChanged();
            }
        }
        public bool BottomBackRight
        {
            get => Direction == ViewCubeCorner.BottomBackRight;
            set
            {
                if (value == true) Direction = ViewCubeCorner.BottomBackRight;
                notifyCornersChanged();
            }
        }
        public bool BottomBackLeft
        {
            get => Direction == ViewCubeCorner.BottomBackLeft;
            set
            {
                if (value == true) Direction = ViewCubeCorner.BottomBackLeft;
                notifyCornersChanged();
            }
        }

        public ViewCubeCorner Direction
        {
            get { return (ViewCubeCorner)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(ViewCubeCorner), typeof(DirectionCtrl_ViewCube), new PropertyMetadata(ViewCubeCorner.None));

    }
}
