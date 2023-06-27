using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;
using System.Windows.Controls;
using AssemblyMgr.UI.Components;

namespace AssemblyMgr.UI.ViewModels
{
    public class ViewPortVM_Schedule : ViewPortVM, INotifyPropertyChanged
    {
        public ViewPortDefinition_Schedule Schedule
        {
            get => base.Definition as ViewPortDefinition_Schedule;
            set => base.Definition = value;
        }

        public ViewPortVM_Schedule(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortDefinition_Schedule()) { }
        public ViewPortVM_Schedule(ViewPortDefinition_Schedule schedule, IAssemblyMgrController controller)
            : base(schedule.Outline, controller, schedule.Type, schedule) { }

        public new event PropertyChangedEventHandler PropertyChanged;

        public override UserControl DirectionControl => new DirectionCtrl_Box2d(); 
    }
}
