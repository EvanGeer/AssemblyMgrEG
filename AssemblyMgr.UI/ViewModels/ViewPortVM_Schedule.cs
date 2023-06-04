using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_Schedule : ViewPortVM, INotifyPropertyChanged
    {
        public new ViewPortSchedule Definition
        {
            get => base.Definition as ViewPortSchedule;
            set => base.Definition = value;
        }

        public ViewPortVM_Schedule(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortSchedule())
        {

        }

        public Quadrant DockPoint 
        {
            get => Definition.DockPoint; 
            set => this.Notify(PropertyChanged, () => Definition.DockPoint = value,
                    alsoNotify: new[] {
                        nameof(TopLeft), 
                        nameof(BottomRight), 
                        nameof(TopRight), 
                        nameof(BottomLeft)
                    }); 
        }
        public bool TopLeft
        {
            get => DockPoint == Quadrant.TopLeft;
            set => DockPoint = Quadrant.TopLeft;
        }
        public bool TopRight
        {
            get => DockPoint == Quadrant.TopRight;
            set => DockPoint = Quadrant.TopRight;
        }
        public bool BottomLeft
        {
            get => DockPoint == Quadrant.BottomLeft;
            set => DockPoint = Quadrant.BottomLeft;
        }
        public bool BottomRight
        {
            get => DockPoint == Quadrant.BottomRight;
            set => DockPoint = Quadrant.BottomRight;
        }

        public new event PropertyChangedEventHandler PropertyChanged;
    }
}
