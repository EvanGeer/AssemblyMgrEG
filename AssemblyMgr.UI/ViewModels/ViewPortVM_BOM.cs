using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_BOM : IViewPort, INotifyPropertyChanged
    {
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public IAssemblyMgrController Controller { get; }

        public ViewPortVM_BOM(ISpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;
            if (SpoolSheetDefinition.BOMFields is null)
                SpoolSheetDefinition.BOMFields = new System.Collections.ObjectModel.ObservableCollection<BOMFieldDefinition>();
            SpoolSheetDefinition.BOMFields.CollectionChanged += BOMFields_CollectionChanged;
        }

        //public List<BOMFieldDefinition> ModelBOMFields { get; set; }
        //     = new List<BOMFieldDefinition>();
        public BOMFieldDefinition CurrnetSelectedBOMField { get; set; }
        public BOMFieldDefinition CurrnetAvailableBOMField { get; set; }
        public IEnumerable<BOMFieldDefinition> BOMFields_Available
            => Controller.GetBOMFields().Where(x => x.PassesSearch(BOMFieldSearch)
                                      && SpoolSheetDefinition?.BOMFields?.Contains(x) != true);
        public string _bomFieldSearch;


        public string BOMFieldSearch
        {
            get => _bomFieldSearch;
            set => this.Notify(PropertyChanged, () => _bomFieldSearch = value,
                alsoNotify: new[] { nameof(BOMFields_Available) });
        }
        public Box2d Outline { get; }
        public ViewPortType Type => ViewPortType.Schedule;

        private void BOMFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                => this.Notify(PropertyChanged, nameof(BOMFields_Available));

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class ViewPortVM_ModelView : IViewPort 
    {
        public bool HasTags { get; set; }
        public bool HasTagLeaders { get; set; }
        public bool HasDimensions { get; set; }
        public Box2d Outline { get; }
        public ViewPortType Type => ViewPortType.ModelElevation; // ToDo: fix this... probably a different interface

        // ToDo: sample image
        // ======================================================
        // - grab a sample spool
        // - show plan/elev/3d per type picked
        // - show dimensions/tags/leaders per toggle state
    }
}
