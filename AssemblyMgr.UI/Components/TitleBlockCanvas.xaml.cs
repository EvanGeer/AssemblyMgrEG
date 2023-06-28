using AssemblyMgr.UI.ViewModels;
using AssemblyMgr.Core.Geometry;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AssemblyMgr.UI.Components
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
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.Source is Image)) return;
            _startPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));
            isAddingRectangle = true;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isAddingRectangle || e.MouseDevice.LeftButton != MouseButtonState.Pressed) return;
            var currentPoint = normalizeToCanvas(e.GetPosition(SheetCanvas));

            var box = new Box2d((_startPoint.X, _startPoint.Y), (currentPoint.X, currentPoint.Y));
            _viewModel.ViewPorts.TempViewPort = (box);
        }

        private Point normalizeToCanvas(Point origin)
        {
            // ToDo: switch back to top left origin,
            // this will fix issues when the canvas is too small
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
            _viewModel.ViewPorts.AddViewPort(deScaled);

            isAddingRectangle = false;
        }

        private void SheetCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var img = System.Drawing.Image.FromFile(_viewModel.ViewPorts.DefaultImage);

            _viewModel.ViewPorts.SheetImageScale = (float)(e.NewSize.Width / ((double)img.Width));
        }

        private void DeleteViewPort(RectangleVM viewPort) 
            => _viewModel.ViewPorts.RemoveViewPort(viewPort);
    }
}
