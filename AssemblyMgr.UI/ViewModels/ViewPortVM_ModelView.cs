using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using System.ComponentModel;
using AssemblyMgr.UI.Extensions;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_ModelView : ViewPortVM, INotifyPropertyChanged
    {
        public new ViewPortModel Definition
        {
            get => base.Definition as ViewPortModel;
            set => base.Definition = value;
        }

        public ViewPortVM_ModelView(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortModel()) { }

        public bool HasTags 
        { 
            get => Definition.HasTags;
            set => this.Notify(PropertyChanged, () => Definition.HasTags = value); 
        }
        public bool HasTagLeaders 
        { 
            get => Definition.HasTagLeaders;
            set => this.Notify(PropertyChanged, () => Definition.HasTagLeaders = value);
        }
        public bool HasDimensions
        {
            get => Definition.HasDimensions;
            set => this.Notify(PropertyChanged, () => Definition.HasDimensions = value);
        }
        public Orientation Orientation
        {
            get => Definition.Orientation;
            set => this.Notify(PropertyChanged, () => Definition.Orientation = value);
        }
        // ToDo: fix this... probably a different interface

        public new event PropertyChangedEventHandler PropertyChanged;

        // ToDo: sample image
        // ======================================================
        // - grab a sample spool
        // - show plan/elev/3d per type picked
        // - show dimensions/tags/leaders per toggle state
    }
}
