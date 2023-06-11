using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using System.ComponentModel;
using AssemblyMgr.UI.Extensions;
using AssemblyMgr.UI.Components;
using System.Windows.Controls;
using System.Windows;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_ModelView : ViewPortVM, INotifyPropertyChanged
    {
        public ViewPortModel ViewPort
        {
            get => Definition as ViewPortModel;
            set => Definition = value;
        }

        public ViewPortVM_ModelView(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortModel()) { }
        public ViewPortVM_ModelView(ViewPortModel model, IAssemblyMgrController controller)
            : base(model.Outline, controller, model.Type, model) { }

        public bool HasTags 
        { 
            get => ViewPort.HasTags;
            set => this.Notify(PropertyChanged, () => ViewPort.HasTags = value); 
        }
        public bool HasTagLeaders 
        { 
            get => ViewPort.HasTagLeaders;
            set => this.Notify(PropertyChanged, () => ViewPort.HasTagLeaders = value);
        }
        public bool HasDimensions
        {
            get => ViewPort.HasDimensions;
            set => this.Notify(PropertyChanged, () => ViewPort.HasDimensions = value);
        }
        //public ElevationOrientation Orientation
        //{
        //    get => ViewPort.Orientation;
        //    set => this.Notify(PropertyChanged, () => ViewPort.Orientation = value);
        //}

        public new event PropertyChangedEventHandler PropertyChanged;

        // ToDo: sample image
        // ======================================================
        // - grab a sample spool
        // - show plan/elev/3d per type picked
        // - show dimensions/tags/leaders per toggle state

        public override UserControl DirectionControl =>
            Type == ViewPortType.ModelElevation
            ? new DirectionCtrl_Compass()
            : Type == ViewPortType.ModelOrtho
            ? new DirectionCtrl_ViewCube()
            : new UserControl { Visibility = Visibility.Collapsed };

        public Visibility DimensionToggleVisibility => 
            Type == ViewPortType.ModelOrtho 
            ? Visibility.Collapsed
            : Visibility.Visible;
    }
}
