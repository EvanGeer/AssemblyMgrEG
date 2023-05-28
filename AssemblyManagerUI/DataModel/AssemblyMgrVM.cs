using AssemblyMgrShared.DataModel;
using AssemblyMgrShared.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AssemblyManagerUI.DataModel
{
    //public enum ViewPortType
    //{
    //    Elevation_Front,
    //    Elevation_Back,
    //    Elevation_Right,
    //    Elevation_Left,
    //    Plan_Top,
    //    Bottom,
    //    Ortho
    //}

    public interface IViewPort
    {
        //int Scale { get; }
        //bool HasTags { get; }
        //bool HasTagLeaders { get; }
        //bool HasDimensions { get; }
        Box2d Outline { get; }
    }

    public class ViewPortVM : INotifyPropertyChanged, IViewPort
    {

        /// <summary>
        /// Defined as percentage relative to sheet
        /// </summary>
        public Box2d Outline
        {
            get => _outline;
            set => this.Notify(PropertyChanged, () => _outline = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }
        public ViewPortVM(Box2d outline, float previewScale)
        {
            Outline = outline;
            _previewScale = previewScale;
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

        bool IsActive
        {
            get => _isActive;
            set => this.Notify(PropertyChanged, () => _isActive = value);
        }


        public float PreviewScale
        {
            get => _previewScale;
            set => this.Notify(PropertyChanged, () => _previewScale = value, alsoNotify: new[] {nameof(PreviewOutline)});
        }
        public Box2d PreviewOutline => Outline is null ? null : new Box2d(Outline.BottomLeft * PreviewScale, Outline.TopRight * PreviewScale);

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public interface ISheetController
    {
        Dictionary<int, string> GetTitleBlocks();
        string GetImage(int titleBlockId);
    }

    public class BillOfMaterialsVM : INotifyPropertyChanged
    {
        // ToDo: implement this
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ViewPortsVM : INotifyPropertyChanged 
    {
        private Box2d _rectangle;// = new Box2d((50, 25), (200, 100));
        public Box2d Rectangle
        {
            get => _rectangle; // == null ? null : new Box2d(_rectangle.BottomLeft * SheetImageScale, _rectangle.TopRight * SheetImageScale);
            set => this.Notify(PropertyChanged, () => _rectangle = value);
        }

        public ObservableCollection<ViewPortVM> Rectangles { get; } = new ObservableCollection<ViewPortVM>
        {
            //new ViewPortVM(new Box2d((50, 25), (200, 100)), 1),
            //new ViewPortVM(new Box2d((550, 255), (200, 100)), 1),
        };

        public string DefaultImage => @"C:\$\Personal\images\TitleBlock - Sheet - 2562563 - 11x17 Titleblock.png";
        private float _sheetImageScale1 = 1;
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

    public class AssemblyMgrVM : INotifyPropertyChanged
    {
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public AssemblyMgrVM(ISpoolSheetDefinition spoolSheetDefinition)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
            SpoolSheetDefinition.BOMFields.CollectionChanged += BOMFields_CollectionChanged;
        }

        public ViewPortsVM ViewPorts { get; set; } = new ViewPortsVM();
        public List<string> TitleBlocks { get; set; }

        public List<BOMFieldDefinition> ModelBOMFields { get; set; }
         = new List<BOMFieldDefinition>();
        public BOMFieldDefinition CurrnetSelectedBOMField { get; set; }
        public BOMFieldDefinition CurrnetAvailableBOMField { get; set; }
        public IEnumerable<BOMFieldDefinition> BOMFields_Available
            => ModelBOMFields.Where(x => x.PassesSearch(BOMFieldSearch)
                                      && SpoolSheetDefinition?.BOMFields?.Contains(x) != true);
        public string _bomFieldSearch;

        public string BOMFieldSearch
        {
            get => _bomFieldSearch;
            set => this.Notify(PropertyChanged, () => _bomFieldSearch = value,
                alsoNotify: new[] { nameof(BOMFields_Available) });
        }



        public string Scale
        {
            get => SpoolSheetDefinition.Scale.ToString();
            set
            {
                // leave value as-is if user input an invalid value
                if (!float.TryParse(value, out float parsedFloat))
                    return;

                // if the user entered a decimal, round it to the nearest int
                int cleanedValue = (int)Math.Round(parsedFloat, 0);

                this.Notify(PropertyChanged, () => SpoolSheetDefinition.Scale = cleanedValue);
            }
        }
        public ObservableCollection<AssemblyViewType> SelectedAssemblyViews { get; }


        public event PropertyChangedEventHandler PropertyChanged;



        private void BOMFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            => this.Notify(PropertyChanged, nameof(BOMFields_Available));
    }

    public sealed class TrulyObservableCollection<T> : ObservableCollection<T>
    where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                this.Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
