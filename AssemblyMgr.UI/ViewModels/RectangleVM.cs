using AssemblyMgr.UI.Components;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Extensions;
using AssemblyMgr.UI.Extensions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace AssemblyMgr.UI.ViewModels
{
    public class RectangleVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ViewPortVM ViewPort { get; set; }
        public SpoolSheetDefinition SpoolSheetDefinition { get; set; }
        public IAssemblyMgrController Controller { get; }

        /// <summary>Defined as percentage relative to sheet</summary>
        public Box2d Outline
        {
            get => _outline;
            set
            {
                this.Notify(PropertyChanged, () => _outline = value,
                 alsoNotify: new[] { nameof(PreviewOutline) });

                if (ViewPort is null) return;
                ViewPort.Definition.Outline = value / Constants.SheetImageWidthPixels;
            }
        }

        public float PreviewScale
        {
            get => _previewScale;
            set => this.Notify(PropertyChanged, () => _previewScale = value, alsoNotify: new[] { nameof(PreviewOutline) });
        }


        public Box2d PreviewOutline => Outline * PreviewScale;

        /// <summary>NOTE: Setting type to None will remove the definition from the Settings object</summary>
        public ViewPortType Type
        {
            get => _type;
            set
            {
                // changing viewport types requires us to switch out our ViewModel
                updateViewport(value);

                this.Notify(PropertyChanged, () => _type = value,
                    alsoNotify: new[]
                    {
                        nameof(MainControl),
                        nameof(GoBackVisibility),
                        nameof(TypeDispaly),
                    });

            }
        }

        public string TypeDispaly => 
            Type.GetAttribute((DescriptionAttribute x) => x.Description);

        public Visibility GoBackVisibility =>
            Type == ViewPortType.None ? Visibility.Collapsed
            : Visibility.Visible;

        public UserControl MainControl =>
            Type == ViewPortType.None ? new ViewPortTypeSelector()
            : Type == ViewPortType.Schedule ? new SheduleViewPort()
            : new ModelViewPort() as UserControl;

        public RectangleVM(ViewPortDefinition viewPort, float previewScale, SpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
            : this(viewPort.Outline, previewScale, spoolSheetDefinition, controller)
        {
            bool isSchedule = viewPort.Type == ViewPortType.Schedule;
            bool isModelView = new[] { ViewPortType.ModelElevation, ViewPortType.ModelPlan, ViewPortType.ModelOrtho }.Contains(viewPort.Type);
            if (isSchedule) ViewPort = new ViewPortVM_Schedule(viewPort as ViewPortDefinition_Schedule, Controller);
            if (isModelView) ViewPort = new ViewPortVM_ModelView(viewPort as ViewPortDefinition_Model, Controller);
            Type = viewPort.Type;
            _outline = viewPort.Outline * Constants.SheetImageWidthPixels;
        }

        public RectangleVM(Box2d outline, float previewScale, SpoolSheetDefinition spoolSheetDefinition, IAssemblyMgrController controller)
        {
            Outline = outline;
            _previewScale = previewScale;
            SpoolSheetDefinition = spoolSheetDefinition;
            Controller = controller;
        }

        private void updateViewport(ViewPortType value)
        {
            var existingViewPortId = ViewPort?.Definition?.Id;

            if (value == ViewPortType.None && existingViewPortId.HasValue)
                removeMatchingDefintions(existingViewPortId.Value, SpoolSheetDefinition.ViewPorts);

            if (value == ViewPortType.None) 
                ViewPort = null;

            if (value != ViewPortType.None && ViewPort is null)
            {
                var newViewPort =
                    value == ViewPortType.None ? null
                    : getNewViewPortViewModel(value, Outline, Controller);

                addNewDefinition(newViewPort, SpoolSheetDefinition.ViewPorts);

                ViewPort = newViewPort;
            }
        }

        private static ViewPortVM getNewViewPortViewModel(ViewPortType value, Box2d outline, IAssemblyMgrController controller)
        {
            bool isSchedule = value == ViewPortType.Schedule;
            bool isModelView = new[] { ViewPortType.ModelElevation, ViewPortType.ModelPlan, ViewPortType.ModelOrtho }.Contains(value);

            var normalizedOutline = outline / 1024f;

            return
                isModelView ? new ViewPortVM_ModelView(normalizedOutline, controller, value)
                : isSchedule ? new ViewPortVM_Schedule(normalizedOutline, controller, value)
                : null as ViewPortVM;
        }


        private static void removeMatchingDefintions(Guid existingId, ObservableCollection<ViewPortDefinition> definitionsToUpdate)
        {
            var existingMatches = definitionsToUpdate
                .Where(x => x.Id == existingId)
                .ToList();

            existingMatches.ForEach(x => definitionsToUpdate.Remove(x));
        }

        private static void addNewDefinition(ViewPortVM newViewModel, ObservableCollection<ViewPortDefinition> definitionsToUpdate)
        {
            if (!(newViewModel?.Definition is ViewPortDefinition definition)) return;

            var existingMatches = definitionsToUpdate
                .Where(x => x.Id == definition.Id)
                .ToList();

            bool addNew = existingMatches.Count == 0;
            if (addNew) definitionsToUpdate.Add(definition);
        }

        private ViewPortType _type = ViewPortType.None;
        private float _previewScale = 1.0f;
        private Box2d _outline;
    }
}
