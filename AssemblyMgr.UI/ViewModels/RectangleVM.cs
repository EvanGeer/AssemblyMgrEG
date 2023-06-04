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
    public class RectangleVM : INotifyPropertyChanged
    {
        public ViewPortVM ViewPort
        {
            get => _viewPort;
            set
            {
                if (!(_viewPort is null) && SpoolSheetDefinition.ViewPorts.Contains(_viewPort?.Definition))
                    SpoolSheetDefinition.ViewPorts.Remove(_viewPort.Definition);

                if (value != null)
                    SpoolSheetDefinition.ViewPorts.Add(value.Definition);

                _viewPort = value;
            }
        }
        public SpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public IAssemblyMgrController Controller { get; }

        /// <summary>
        /// Defined as percentage relative to sheet
        /// </summary>
        public Box2d Outline
        {
            get => _outline;
            set => this.Notify(PropertyChanged, () => _outline = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }
        public RectangleVM(Box2d outline, float previewScale, SpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            Outline = outline;
            _previewScale = previewScale;
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;
        }

        //private string _name;
        //public string Name
        //{
        //    get => _name;
        //    set => this.Notify(PropertyChanged, () => _name = value);
        //}

        private bool _isActive;
        private Box2d _outline;
        private float _previewScale = 1.0f;
        private ViewPortType _type = ViewPortType.None;
        private ViewPortVM _viewPort;

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
                if (isSchedule) ViewPort = new ViewPortVM_Schedule(new Box2d(Outline.BottomLeft/1024f, Outline.TopRight/1024f), Controller, value);
                if (isModelView) ViewPort = new ViewPortVM_ModelView(new Box2d(Outline.BottomLeft / 1024f, Outline.TopRight / 1024f), Controller, value);
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
            Type == ViewPortType.None ? Visibility.Collapsed
            : Visibility.Visible;

        public UserControl MainControl =>
            Type == ViewPortType.None ? new ViewPortTypeSelector()
            : Type == ViewPortType.Schedule ? new SheduleViewPort()
            : new ModelViewPort() as UserControl;
    }
}
