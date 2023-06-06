using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.UI.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;

namespace AssemblyMgr.UI.ViewModels
{
    public class SheetLayoutVM : INotifyPropertyChanged
    {
        public SpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public IAssemblyMgrController Controller { get; }

        public Visibility TempViewPortVisibility => TempViewPort is null ? Visibility.Collapsed : Visibility.Visible;
        private Box2d _tempViewPort;
        public Box2d TempViewPort
        {
            get => _tempViewPort;
            set => this.Notify(PropertyChanged, () => _tempViewPort = value, alsoNotify: new[] { nameof(TempViewPortVisibility) });
        }



        public ObservableCollection<RectangleVM> Rectangles { get; } = new ObservableCollection<RectangleVM>
        {
            //new RectangleVM(new Box2d((50, 25), (200, 100)), 1),
            //new RectangleVM(new Box2d((550, 255), (200, 100)), 1),
        };

        public void AddViewPort(ViewPortDefinition viewPort)
        {
            Rectangles.Add(new RectangleVM(viewPort, SheetImageScale, SpoolSheetDefinition, Controller));
        }
        public void AddViewPort(Box2d rectangle)
        {
            Rectangles.Add(new RectangleVM(rectangle, SheetImageScale, SpoolSheetDefinition, Controller));
            TempViewPort = null;
        }

        public string DefaultImage => @"C:\$\Personal\images\TitleBlock - Sheet - 2562563 - 11x17 Titleblock.png";
        private float _sheetImageScale1 = 1;

        public SheetLayoutVM(SpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;

            if (spoolSheetDefinition?.ViewPorts?.Count > 0)
            {
                var existinViewPorts = spoolSheetDefinition.ViewPorts.ToList();
                existinViewPorts.ForEach(x => AddViewPort(x));
                this.Notify(PropertyChanged, nameof(Rectangles));
            }
        }

        public float SheetImageScale
        {
            get => _sheetImageScale1;
            set => this.Notify(PropertyChanged, () =>
            {
                _sheetImageScale1 = value;
                foreach (RectangleVM v in Rectangles) v.PreviewScale = _sheetImageScale1;
            },
            alsoNotify: new[] { nameof(TempViewPort), nameof(Rectangles) });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
