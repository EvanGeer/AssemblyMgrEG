using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;
using System.Windows.Controls;

namespace AssemblyMgr.UI.ViewModels
{
    public abstract class ViewPortVM : INotifyPropertyChanged
    {   
        public ViewPortDefinition Definition { get; set; }
        public IAssemblyMgrController Controller { get; }

        public ViewPortVM(Box2d outline, IAssemblyMgrController controller, ViewPortType type, ViewPortDefinition definition)
        {
            Definition = definition;

            Controller = controller;
            Type = type;
            Definition.Outline = outline;
        }

        public ViewPortType Type
        {
            get => Definition.Type;
            set => this.Notify(PropertyChanged, () => Definition.Type = value);
        }
        public bool IgnoreWelds
        {
            get => Definition.IgnoreWelds;
            set => this.Notify(PropertyChanged, () => Definition.IgnoreWelds = value);
        }
        public string Title
        {
            get => Definition.Title;
            set => this.Notify(PropertyChanged, () => Definition.Title = value);
        }
        public string ViewTemplate
        {
            get => Definition?.ViewTemplate;
            set => this.Notify(PropertyChanged, () => Definition.ViewTemplate = value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public abstract UserControl DirectionControl { get; }

    }
}
