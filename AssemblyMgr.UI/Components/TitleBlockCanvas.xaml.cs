using AssemblyManagerUI.ViewModels;
using AssemblyMgrShared.DataModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssemblyManagerUI.Components
{
    /// <summary>
    /// Interaction logic for TitleBlockCanvas.xaml
    /// </summary>
    public partial class TitleBlockCanvas : UserControl
    {

        public TitleBlockCanvas()
        {
            InitializeComponent();
            this.DataContextChanged += setViewModel;
        }

        private void setViewModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as AssemblyMgrVM;
        }

        private AssemblyMgrVM _viewModel;
        bool isAddingRectangle = false;
        private Point _startPoint;
        //private Box2d _currentbBox;
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is Image)) return;
            _startPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));
            isAddingRectangle = true;
            //this._viewModel.TempViewPort.
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isAddingRectangle || e.MouseDevice.LeftButton != MouseButtonState.Pressed) return;
            var currentPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));
            //currentPoint.Y = SheetCanvas.ActualHeight - currentPoint.Y;
            var box = new Box2d((_startPoint.X, _startPoint.Y), (currentPoint.X, currentPoint.Y));
            _viewModel.ViewPorts.TempViewPort = (box);
        }

        private Point normalizeToCanvas(Point origin)
        {
            // convert bottom left origin
            // ToDo: switch back to top left origin
            var bl_origin = new Point(origin.X, SheetCanvas.ActualHeight - origin.Y);

            return bl_origin;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isAddingRectangle) return;

            if (_viewModel.ViewPorts.TempViewPort == null) return;

            var newRectangle = _viewModel.ViewPorts.TempViewPort;
            var deScaled = new Box2d(newRectangle.BottomLeft / _viewModel.ViewPorts.SheetImageScale,
                newRectangle.TopRight / _viewModel.ViewPorts.SheetImageScale);
            _viewModel.ViewPorts.Rectangles.Add(new ViewPortVM(deScaled, _viewModel.ViewPorts.SheetImageScale, _viewModel.SpoolSheetDefinition));
            _viewModel.ViewPorts.TempViewPort = null;
            isAddingRectangle = false;
        }

        private void SheetCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var img = System.Drawing.Image.FromFile(_viewModel.ViewPorts.DefaultImage);
            //var imageSize =  img.Width + ", Height: " + img.Height);

            _viewModel.ViewPorts.SheetImageScale = (float)(e.NewSize.Width / ((double)img.Width));
        }


        private void DeleteCard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.ViewPorts.Rectangles.Remove((sender as FrameworkElement)?.DataContext as ViewPortVM);
        }

        private void DeleteViewPort(ViewPortVM viewPort) => _viewModel.ViewPorts.Rectangles.Remove(viewPort);

    }
}
