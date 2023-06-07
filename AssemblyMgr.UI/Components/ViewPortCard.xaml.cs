using AssemblyMgr.Core.DataModel;
using AssemblyMgr.Core.Geometry;
using AssemblyMgr.UI.ViewModels;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AssemblyMgr.UI.Components
{
    /// <summary>
    /// Interaction logic for ViewPortCard.xaml
    /// </summary>
    public partial class ViewPortCard : UserControl
    {
        public ViewPortCard()
        {
            InitializeComponent();
            this.DataContextChanged += setViewModel;
        }

        private void setViewModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = this.DataContext as RectangleVM;
            //this.Height = _viewModel.PreviewOutline.Height;
            //this.Width = _viewModel.PreviewOutline.Width;
        }

        private RectangleVM _viewModel;



        private void DeleteCard_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (OnDeleted is null) return;
            OnDeleted((sender as FrameworkElement)?.DataContext as RectangleVM);
        }

        public Action<RectangleVM> OnDeleted
        {
            get { return (Action<RectangleVM>)GetValue(OnDeletedProperty); }
            set { SetValue(OnDeletedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OnDeleted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnDeletedProperty =
            DependencyProperty.Register("OnDeleted", typeof(Action<RectangleVM>), typeof(ViewPortCard), new PropertyMetadata(null));

        private void GoBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _viewModel.Type = ViewPortType.None;
        }


        private Box2d _resizeBox = null;
        private Box2dLocation _oppositeCorner = Box2dLocation.None;
        private Box2dLocation _originalCorner = Box2dLocation.None;
        private Vector2 _startingMouse = Vector2.Zero;

        private void BottomLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _resizeBox = new Box2d(_viewModel.Outline.BottomLeft, _viewModel.Outline.TopRight);
            _oppositeCorner = Box2dLocation.TopRight;
            _originalCorner = Box2dLocation.BottomLeft;
            var mPos = e.GetPosition(this);
            _startingMouse = new Vector2((float)mPos.X, (float)mPos.Y);
        }

        private void BottomRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _resizeBox = new Box2d(_viewModel.Outline.BottomLeft, _viewModel.Outline.TopRight);
            _oppositeCorner = Box2dLocation.TopLeft;
            _originalCorner = Box2dLocation.BottomRight;
            var mPos = e.GetPosition(this);
            _startingMouse = new Vector2((float)mPos.X, (float)mPos.Y);
        }

        private void ResizeCard(object sender, MouseEventArgs e)
        {
            if (_resizeBox is null) return;

            var oppositeCorner = _resizeBox.GetCoordinate(_oppositeCorner);
            var originalCorner = _resizeBox.GetCoordinate(_originalCorner);

            var mouseLocationPoint = e.GetPosition(this);
            var newLocation = new Vector2((float)mouseLocationPoint.X, (float)mouseLocationPoint.Y);
            var translation = newLocation - _startingMouse;
            var newPosition = translation + originalCorner;
            _viewModel.Outline = new Box2d(newPosition / _viewModel.PreviewScale, oppositeCorner / _viewModel.PreviewScale);

        }

        private void ResizeDone(object sender, MouseButtonEventArgs e)
        {
            _resizeBox = null;
        }

        private void myThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //Move the Thumb to the mouse position during the drag operation
            var translation = new Vector2((float)e.HorizontalChange, (-1.0f)*(float)e.VerticalChange);
            _viewModel.Outline = new Box2d(_viewModel.Outline.BottomLeft +  translation, _viewModel.Outline.TopRight + translation);

            //double yadjust = this.Height + e.VerticalChange;
            //double xadjust = this.Width + e.HorizontalChange;
            //if ((xadjust >= 0) && (yadjust >= 0))
            //{
            //    this.Width = xadjust;
            //    this.Height = yadjust;
            //    Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) +
            //                            e.HorizontalChange);
            //    Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) +
            //                            e.VerticalChange);
            //    //changes.Text = "Size: " +
            //    //                this.Width.ToString() +
            //    //                 ", " +
            //    //                this.Height.ToString();
            //}
        }

        private void myThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
        }

        private void myThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            myThumb.Background = Brushes.Blue;
        }
    }
}
