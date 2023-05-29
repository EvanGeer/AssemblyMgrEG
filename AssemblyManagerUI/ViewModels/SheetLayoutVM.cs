using AssemblyMgrShared.DataModel;
using AssemblyMgrShared.UI;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AssemblyManagerUI.ViewModels
{
    public class SheetLayoutVM : INotifyPropertyChanged 
    {
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }
        private Box2d _rectangle;// = new Box2d((50, 25), (200, 100));
        public Box2d Rectangle
        {
            get => _rectangle; // == null ? null : new Box2d(_rectangle.BottomLeft * SheetImageScale, _rectangle.TopRight * SheetImageScale);
            set => this.Notify(PropertyChanged, () => _rectangle = value);
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
                    alsoNotify: new[] { nameof(Rectangle), nameof(Rectangles) });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
