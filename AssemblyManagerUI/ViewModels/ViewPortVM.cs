using AssemblyManagerUI.Components;
using AssemblyMgrShared.DataModel;
using AssemblyMgrShared.UI;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;

namespace AssemblyManagerUI.ViewModels
{
    public class ViewPortVM : INotifyPropertyChanged, IViewPort
    {
        public IViewPort ViewPortProps { get; set; }
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }

        /// <summary>
        /// Defined as percentage relative to sheet
        /// </summary>
        public Box2d Outline
        {
            get => _outline;
            set => this.Notify(PropertyChanged, () => _outline = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }
        public ViewPortVM(Box2d outline, float previewScale, ISpoolSheetDefinition spoolSheetDefinition)
        {
            Outline = outline;
            _previewScale = previewScale;
            SpoolSheetDefinition = spoolSheetDefinition;
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
                if (isSchedule) ViewPortProps = new ViewPortVM_BOM(SpoolSheetDefinition);
                if (isModelView) ViewPortProps = new ViewPortVM_ModelView();
                this.Notify(PropertyChanged, () => _type = value, alsoNotify: new[] { nameof(MainControl) });
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public UserControl MainControl => 
            Type == ViewPortType.None ? null :
            Type == ViewPortType.Schedule
            ? new SheduleViewPort() as UserControl
            : new ModelViewPort() as UserControl;
    }
}
