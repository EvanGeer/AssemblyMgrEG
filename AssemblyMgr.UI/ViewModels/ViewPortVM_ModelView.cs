using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using System.ComponentModel;
using AssemblyMgr.UI.Extensions;
using AssemblyMgr.UI.Components;
using System.Windows.Controls;
using System.Windows;
using AssemblyMgr.Core.Extensions;
using System.Linq;
using System.Windows.Media;
using System.Collections.Generic;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_ModelView : ViewPortVM, INotifyPropertyChanged
    {
        public ViewPortDefinition_Model ViewPort
        {
            get => Definition as ViewPortDefinition_Model;
            set => Definition = value;
        }

        public ViewPortVM_ModelView(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortDefinition_Model()) { }
        public ViewPortVM_ModelView(ViewPortDefinition_Model model, IAssemblyMgrController controller)
            : base(model.Outline, controller, model.Type, model) { }

        public bool HasTags
        {
            get => ViewPort.HasTags;
            set => this.Notify(PropertyChanged, () => ViewPort.HasTags = value,
                    alsoNotify: new[] 
                    { 
                        nameof(TagSettingsDisplay),  
                        nameof(TagSettingsForeground),
                    });
        }
        public string JointTag
        {
            get => ViewPort.JointTag;
            set => this.Notify(PropertyChanged, () => ViewPort.JointTag = value,
                    alsoNotify: new[] { nameof(TagSettingsDisplay) });
        }
        public string PipeTag
        {
            get => ViewPort.PipeTag;
            set => this.Notify(PropertyChanged, () => ViewPort.PipeTag = value,
                    alsoNotify: new[] { nameof(TagSettingsDisplay) });
        }

        public string FittingTag
        {
            get => ViewPort.FittingTag;
            set => this.Notify(PropertyChanged, () => ViewPort.FittingTag = value,
                    alsoNotify: new[] { nameof(TagSettingsDisplay) });
        }

        public string TagSettingsDisplay
            => HasTags 
            && ItemsToTag != ItemType.None 
            && ItemsToTag.GetFlags(x => x != ItemType.None).ToList() is List<ItemType> items
            ? items.Count == 3 ? "All Items" : string.Join(", ", ItemsToTag.GetFlags().Except(new[] {ItemType.None}))
            : "None";
        public Brush TagSettingsForeground => HasTags
            ? Brushes.Black : Brushes.LightGray;
        public Brush TagSettingsBorderBrush => HasTags
            ? Brushes.LightSteelBlue : Brushes.LightGray;
        public ItemType ItemsToTag
        {
            get => ViewPort.ItemsToTag;
            set => this.Notify(PropertyChanged, () => ViewPort.ItemsToTag = value,
                    alsoNotify: new[] { nameof(TagSettingsDisplay) });
        }
        public bool HasDimensions
        {
            get => ViewPort.HasDimensions;
            set => this.Notify(PropertyChanged, () => ViewPort.HasDimensions = value);
        }

        public bool HasPipeTags
        {
            get => ItemsToTag.HasFlag(ItemType.Pipe);
            set => ItemsToTag = value == true
                ? ItemsToTag | ItemType.Pipe  & ~ItemType.None
                : ItemsToTag & ~ItemType.Pipe;
        }
        public bool HasJointTags
        {
            get => ItemsToTag.HasFlag(ItemType.Joint);
            set => ItemsToTag = value == true
                ? ItemsToTag | ItemType.Joint & ~ItemType.None
                : ItemsToTag & ~ItemType.Joint;
        }
        public bool HasFittingTags
        {
            get => ItemsToTag.HasFlag(ItemType.Fitting);
            set => ItemsToTag = value == true
                ? ItemsToTag | ItemType.Fitting & ~ItemType.None
                : ItemsToTag & ~ItemType.Fitting;
        }

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
