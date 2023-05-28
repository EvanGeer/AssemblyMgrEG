using AssemblyManagerUI.DataModel;
using AssemblyMgrShared.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AssemblyManagerUI
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class AssemblyMgrForm : Window
    {
        public AssemblyMgrVM AssemblyData { get; set; }
        public bool Run { get; set; } = false;

        public AssemblyMgrForm(AssemblyMgrVM assemblyData)
        {
            DataContext = assemblyData;
            AssemblyData = assemblyData;
            Title = "Assembly Manager: " + AssemblyData?.SpoolSheetDefinition?.AssemblyName;

            InitializeComponent();
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            var validation = ValidateInputs();
            if (!validation.passed)
            {
                MessageBox.Show(validation.message);
                return;
            }

            Run = true;
            Close();
        }


        private List<(Predicate<ISpoolSheetDefinition> isValid, string errorMesssage)> inputValidations
            = new List<(Predicate<ISpoolSheetDefinition> isValid, string errorMesssage)>
            {
                (x => !string.IsNullOrEmpty(x.TitleBlock), "A TitelBlock is required."),
                (x => x.BOMFields.Count > 0, "Please pick at least one column for the Bill of Materials."),
                (x => x.Scale > 0, "Please select a valid scale.")
            };

        private (bool passed, string message) ValidateInputs()
        {
            var inputErrors = inputValidations
                .Where(x => !x.isValid(AssemblyData.SpoolSheetDefinition))
                .ToList();

            bool validationPassed = inputErrors.Count == 0;
            string message = validationPassed
                ? string.Empty
                : $"One or more validation failed:" +
                    $"\n   - {string.Join("\n   - ", inputErrors.Select(x => x.errorMesssage))}";

            return (validationPassed, message);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AssemblyData.SpoolSheetDefinition.BOMFields.Add(AssemblyData.CurrnetAvailableBOMField);
        }

        private void Rem_Click(object sender, RoutedEventArgs e)
        {
            AssemblyData.SpoolSheetDefinition.BOMFields.Remove(AssemblyData.CurrnetSelectedBOMField);
        }

        private Point _startPoint;
        //private Box2d _currentbBox;
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));
            //this.AssemblyData.Rectangle.
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MouseDevice.LeftButton != MouseButtonState.Pressed) return;
            var currentPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));
            //currentPoint.Y = SheetCanvas.ActualHeight - currentPoint.Y;
            AssemblyData.ViewPorts.Rectangle = (new Box2d(_startPoint, currentPoint));
        }

        private Point normalizeToCanvas(Point origin)
        {
            // convert bottom left origin
            var bl_origin = new Point(origin.X, SheetCanvas.ActualHeight - origin.Y);

            // remove scaling
            //var de_scaled = new Point(bl_origin.X / AssemblyData.ViewPorts.SheetImageScale,
            //    bl_origin.Y / AssemblyData.ViewPorts.SheetImageScale);
            //return de_scaled;
            return bl_origin;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (AssemblyData.ViewPorts.Rectangle == null) return;

            var newRectangle = AssemblyData.ViewPorts.Rectangle;
            var deScaled = new Box2d(newRectangle.BottomLeft / AssemblyData.ViewPorts.SheetImageScale,
                newRectangle.TopRight / AssemblyData.ViewPorts.SheetImageScale);
            AssemblyData.ViewPorts.Rectangles.Add(new ViewPortVM(deScaled, AssemblyData.ViewPorts.SheetImageScale));
            AssemblyData.ViewPorts.Rectangle = null;
        }

        private void SheetCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var img = System.Drawing.Image.FromFile(AssemblyData.ViewPorts.DefaultImage);
            //var imageSize =  img.Width + ", Height: " + img.Height);

            AssemblyData.ViewPorts.SheetImageScale = (float)(e.NewSize.Width / ((double)img.Width));
        }
    }
}
