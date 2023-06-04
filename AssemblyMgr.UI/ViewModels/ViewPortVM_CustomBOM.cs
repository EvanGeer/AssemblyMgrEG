using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_CustomBOM : ViewPortVM, INotifyPropertyChanged
    {
        public new ViewPortCustomBom Definition
        {
            get => base.Definition as ViewPortCustomBom;
            set => base.Definition = value;
        }

        public ViewPortVM_CustomBOM(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortCustomBom())
        {
            Definition.BOMFields = new System.Collections.ObjectModel.ObservableCollection<BOMFieldDefinition>();
            Definition.BOMFields.CollectionChanged += BOMFields_CollectionChanged;
        }

        public BOMFieldDefinition CurrnetSelectedBOMField { get; set; }
        public BOMFieldDefinition CurrnetAvailableBOMField { get; set; }
        public IEnumerable<BOMFieldDefinition> BOMFields_Available
            => Controller.BOMFields.Where(x => x.PassesSearch(BOMFieldSearch)
                                      && Definition?.BOMFields?.Contains(x) != true);
        public string _bomFieldSearch;


        public string BOMFieldSearch
        {
            get => _bomFieldSearch;
            set => this.Notify(PropertyChanged, () => _bomFieldSearch = value,
                alsoNotify: new[] { nameof(BOMFields_Available) });
        }

        private void BOMFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                => this.Notify(PropertyChanged, nameof(BOMFields_Available));

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
