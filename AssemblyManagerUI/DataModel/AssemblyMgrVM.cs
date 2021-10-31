using AssemblyMgrShared.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AssemblyManagerUI.DataModel
{
    public class AssemblyMgrVM : INotifyPropertyChanged
    {
        public ISpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public List<string> TitleBlocks { get; set; }

        public List<BOMFieldDefinition> BOMFields_All { get; set; }
        public BOMFieldDefinition CurrnetSelectedBOMField { get; set; }
        public BOMFieldDefinition CurrnetAvailableBOMField { get; set; }
        public IEnumerable<BOMFieldDefinition> BOMFields_Available
            => BOMFields_All.Where(x => x.PassesSearch(BOMFieldSearch)
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


        public AssemblyMgrVM(ISpoolSheetDefinition spoolSheetDefinition)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
            SpoolSheetDefinition.BOMFields.CollectionChanged += BOMFields_CollectionChanged;
        }

        private void BOMFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) 
            => this.Notify(PropertyChanged, nameof(BOMFields_Available));
    }
}
