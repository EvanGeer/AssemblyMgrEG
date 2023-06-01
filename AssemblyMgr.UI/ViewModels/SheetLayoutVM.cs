using AssemblyMgr.Core.Geometry;
using AssemblyMgrShared.DataModel;
using AssemblyMgrShared.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace AssemblyManagerUI.ViewModels
{
    public class SheetLayoutVM : INotifyPropertyChanged 
    {
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }


        public Visibility TempViewPortVisibility => TempViewPort is null ? Visibility.Collapsed : Visibility.Visible;
        private Box2d _tempViewPort;
        public Box2d TempViewPort
        {
            get => _tempViewPort;
            set => this.Notify(PropertyChanged, () => _tempViewPort = value, alsoNotify: new[] { nameof(TempViewPortVisibility) });
        }



        public ObservableCollection<IViewPort> Rectangles { get; } = new ObservableCollection<IViewPort>
        {
            //new ViewPortVM(new Box2d((50, 25), (200, 100)), 1),
            //new ViewPortVM(new Box2d((550, 255), (200, 100)), 1),
        };

        public string DefaultImage => @"C:\$\Personal\images\TitleBlock - Sheet - 2562563 - 11x17 Titleblock.png";
        private float _sheetImageScale1 = 1;

        public SheetLayoutVM(ISpoolSheetDefinition spoolSheetDefinition)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
        }

        public float SheetImageScale
        {
            get => _sheetImageScale1;
            set => this.Notify(PropertyChanged, () => {
                    _sheetImageScale1 = value;
                    foreach(ViewPortVM v in Rectangles) v.PreviewScale = _sheetImageScale1;
                },
                    alsoNotify: new[] { nameof(TempViewPort), nameof(Rectangles) });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
