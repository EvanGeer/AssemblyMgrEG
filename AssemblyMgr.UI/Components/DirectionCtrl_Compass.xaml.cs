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
        public DirectionCtrl_Compass()
        {
            InitializeComponent();
        }
    }
}
