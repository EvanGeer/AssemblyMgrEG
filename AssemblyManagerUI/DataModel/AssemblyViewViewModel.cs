using AssemblyMgrCore.UI;
using System.ComponentModel;

namespace AssemblyManagerUI.DataModel
{
    public class AssemblyViewViewModel : AssemblyViewDefinition, INotifyPropertyChanged
    {
        public new AssemblyViewType ViewType
        {
            get => base.ViewType;
            set => this.Notify(PropertyChanged, () => base.ViewType = value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => this.Notify(PropertyChanged, () => _isSelected = value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
