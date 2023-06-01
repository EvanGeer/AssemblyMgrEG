using AssemblyMgr.UI.Components;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM : INotifyPropertyChanged, IViewPort
    {
        public IViewPort ViewPortProps { get; set; }
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public IAssemblyMgrController Controller { get; }

        /// <summary>
        /// Defined as percentage relative to sheet
        /// </summary>
        public Box2d Outline
        {
            get => _outline;
            set => this.Notify(PropertyChanged, () => _outline = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }
        public ViewPortVM(Box2d outline, float previewScale, ISpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            Outline = outline;
            _previewScale = previewScale;
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => this.Notify(PropertyChanged, () => _name = value);
        }

        private bool _isActive;
        private Box2d _outline;
        private float _previewScale = 1.0f;
        private ViewPortType _type = ViewPortType.None;

        bool IsActive
        {
            get => _isActive;
            set => this.Notify(PropertyChanged, () => _isActive = value);
        }


        public float PreviewScale
        {
            get => _previewScale;
            set => this.Notify(PropertyChanged, () => _previewScale = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }
        public Box2d PreviewOutline => Outline is null ? null : new Box2d(Outline.BottomLeft * PreviewScale, Outline.TopRight * PreviewScale);

        public ViewPortType Type
        {
            get => _type;
            set
            {
                bool isSchedule = value == ViewPortType.Schedule;
                bool isModelView = new[] { ViewPortType.ModelElevation, ViewPortType.ModelPlan, ViewPortType.ModelOrtho }.Contains(value);
                if (isSchedule) ViewPortProps = new ViewPortVM_BOM(SpoolSheetDefinition, Controller);
                if (isModelView) ViewPortProps = new ViewPortVM_ModelView();
                this.Notify(PropertyChanged, () => _type = value, 
                    alsoNotify: new[] 
                    { 
                        nameof(MainControl), 
                        nameof(GoBackVisibility),
                        nameof(TypeDispaly),
                    });
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public string TypeDispaly => Type.GetAttribute((DescriptionAttribute x) => x.Description);

        public Visibility GoBackVisibility => 
            Type == ViewPortType.None  ? Visibility.Collapsed 
            : Visibility.Visible;

        public UserControl MainControl => 
            Type == ViewPortType.None ? new ViewPortTypeSelector()
            : Type == ViewPortType.Schedule ? new SheduleViewPort()
            : new ModelViewPort() as UserControl;
    }
}
