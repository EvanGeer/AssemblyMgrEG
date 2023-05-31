using AssemblyMgrShared.DataModel;
using AssemblyMgrShared.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace AssemblyManagerUI.ViewModels
{
    public interface ISheetController
    {
        Dictionary<int, string> GetTitleBlocks();
        string GetImage(int titleBlockId);
    }

    public class AssemblyMgrVM : INotifyPropertyChanged
    {
        private TitleBlock _titleBlock;

        public ISpoolSheetDefinition SpoolSheetDefinition { get; }
        public IAssemblyMgrController Controller { get; }

        public AssemblyMgrVM(ISpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;
            ViewPorts = new SheetLayoutVM(SpoolSheetDefinition);
        }

        public SheetLayoutVM ViewPorts { get; set; }

        public List<string> TitleBlocks => Controller.TitleBlockController.GetTitleBlocks();
        public string TitleBlock
        {
            get => SpoolSheetDefinition.TitleBlock; 
            set => this.Notify(PropertyChanged, () => SpoolSheetDefinition.TitleBlock = value,
                    alsoNotify: new[] { nameof(TitleblockImagePath) }); 
        }

        public string TitleblockImagePath => Controller.TitleBlockController
            .GetTitleBlockImage(TitleBlock);

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



    }

}
