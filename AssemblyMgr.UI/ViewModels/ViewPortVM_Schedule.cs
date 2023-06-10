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
        public ViewPortSchedule Schedule
        {
            get => base.Definition as ViewPortSchedule;
            set => base.Definition = value;
        }

        public ViewPortVM_Schedule(Box2d outline, IAssemblyMgrController controller, ViewPortType type)
            : base(outline, controller, type, new ViewPortSchedule()) { }
        public ViewPortVM_Schedule(ViewPortSchedule schedule, IAssemblyMgrController controller)
            : base(schedule.Outline, controller, schedule.Type, schedule) { }

        public new event PropertyChangedEventHandler PropertyChanged;

        public override UserControl DirectionControl => new DirectionCtrl_Box2d(); 
    }
}
