using MaterialLibs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class DraggedBadge : ContentControl
    {
        public DraggedBadge()
        {
            this.DefaultStyleKey = typeof(DraggedBadge);
            this.Loaded += _Loaded;
            this.AddHandler(PointerPressedEvent, new PointerEventHandler(_PointerPressed), true);
            this.AddHandler(PointerReleasedEvent, new PointerEventHandler(_PointerReleased), true);
            this.AddHandler(PointerCanceledEvent, new PointerEventHandler(_PointerCanceled), true);
            this.AddHandler(PointerMovedEvent, new PointerEventHandler(_PointerMoved), true);

            _GestureRecognizer = new GestureRecognizer();
            _GestureRecognizer.GestureSettings = GestureSettings.ManipulationTranslateX | GestureSettings.ManipulationTranslateY;
            _GestureRecognizer.ManipulationStarted += _ManipulationStarted;
            _GestureRecognizer.ManipulationUpdated += _ManipulationUpdate;
            _GestureRecognizer.ManipulationCompleted += _ManipulationCompleted;
        }

        private Rectangle BackgroundRectangle;
        private Rectangle PopupBackgroundRectangle;
        private Grid ContentGrid;
        private Grid PopupContentGrid;
        private Canvas DragCanvas;
        private PathFigure PathFigure1;
        private QuadraticBezierSegment Bezier1;
        private QuadraticBezierSegment Bezier2;
        private LineSegment Line1;
        private CompositeTransform BaseCircleTrans;
        private TranslateTransform ContentGridTrans;
        private PointTracker TargetTracker;
        private PointTracker SourceTracker;
        private Popup ContentPopup;
        private Grid TopMostContentRoot;

        private Storyboard TargetMoveComplateAnimation;
        private PointAnimation _TargetPointAnimation;
        private Storyboard SourceMoveComplateAnimation;
        private PointAnimation _SourcePointAnimation;

        private GestureRecognizer _GestureRecognizer;

        private bool IsOverflow;
        private bool IsDragging;
        private bool IsOverflowAnimating;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BackgroundRectangle = GetTemplateChild("BackgroundRectangle") as Rectangle;
            PopupBackgroundRectangle = GetTemplateChild("PopupBackgroundRectangle") as Rectangle;
            ContentGrid = GetTemplateChild("ContentGrid") as Grid;
            PopupContentGrid = GetTemplateChild("PopupContentGrid") as Grid;
            DragCanvas = GetTemplateChild("DragCanvas") as Canvas;
            PathFigure1 = GetTemplateChild("PathFigure1") as PathFigure;
            Bezier1 = GetTemplateChild("Bezier1") as QuadraticBezierSegment;
            Bezier2 = GetTemplateChild("Bezier2") as QuadraticBezierSegment;
            Line1 = GetTemplateChild("Line1") as LineSegment;
            BaseCircleTrans = GetTemplateChild("BaseCircleTrans") as CompositeTransform;
            ContentGridTrans = GetTemplateChild("ContentGridTrans") as TranslateTransform;
            TargetTracker = GetTemplateChild("TargetTracker") as PointTracker;
            SourceTracker = GetTemplateChild("SourceTracker") as PointTracker;
            ContentPopup = GetTemplateChild("ContentPopup") as Popup;
            TopMostContentRoot = GetTemplateChild("TopMostContentRoot") as Grid;

            if (BackgroundRectangle != null)
            {
                BackgroundRectangle.SizeChanged += BackgroundRectangle_SizeChanged;
            }
        }

        private void _ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            IsOverflow = false;
            IsOverflowAnimating = false;
            IsDragging = true;
            VisualStateManager.GoToState(this, "Dragging", false);
        }

        private void _ManipulationUpdate(object sender, ManipulationUpdatedEventArgs e)
        {
            TargetTracker.Position = e.Cumulative.Translation;
        }

        private void _ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            Judge();
        }

        private void TargetTracker_PositionChanged(object sender, PositionChangedEventArgs args)
        {
            UpdatePosition();
        }

        private void SourceTracker_PositionChanged(object sender, PositionChangedEventArgs args)
        {
            UpdatePosition();
            if (BaseCircleTrans != null)
            {
                BaseCircleTrans.TranslateX = args.Position.X;
                BaseCircleTrans.TranslateY = args.Position.Y;
            }
        }

        private void TargetMoveComplateAnimation_Completed(object sender, object e)
        {
            ContentPopup.IsOpen = false;
        }

        private void SourceMoveComplateAnimation_Completed(object sender, object e)
        {
            IsOverflowAnimating = false;
            SourceTracker.Position = new Point(0, 0);
            SetBezier1(new Point(0d, 0d), new Point(0d, 0d), new Point(0d, 0d));
            SetBezier2(new Point(0d, 0d), new Point(0d, 0d), new Point(0d, 0d));
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            SetupTracker();
        }

        private void BackgroundRectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (BackgroundRectangle != null)
            {
                var radius = Math.Min(e.NewSize.Width, e.NewSize.Height) / 2;
                BackgroundRectangle.RadiusX = radius;
                BackgroundRectangle.RadiusY = radius;
                PopupBackgroundRectangle.RadiusX = radius;
                PopupBackgroundRectangle.RadiusY = radius;
            }
        }

        private void _PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ContentPopup.Width = this.ActualWidth;
            ContentPopup.Height = this.ActualHeight;
            PopupContentGrid.Width = ContentGrid.ActualWidth;
            PopupContentGrid.Height = ContentGrid.ActualHeight;
            this.CapturePointer(e.Pointer);
            _GestureRecognizer.ProcessDownEvent(e.GetCurrentPoint(ContentGrid));
            ContentPopup.IsOpen = true;
        }

        private void _PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _GestureRecognizer.ProcessMoveEvents(e.GetIntermediatePoints(ContentGrid));
        }

        private void _PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _GestureRecognizer.ProcessUpEvent(e.GetCurrentPoint(ContentGrid));
            this.ReleasePointerCapture(e.Pointer);
            if(TargetTracker.Position.X == 0 && TargetTracker.Position.Y == 0)
            {
                ContentPopup.IsOpen = false;
            }
        }

        private void _PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            _GestureRecognizer.ProcessUpEvent(e.GetCurrentPoint(ContentGrid));
            if (TargetTracker.Position.X == 0 && TargetTracker.Position.Y == 0)
            {
                ContentPopup.IsOpen = false;
            }
        }

        private void SetupTracker()
        {
            if (TargetTracker != null)
            {
                TargetTracker.PositionChanged += TargetTracker_PositionChanged;

                _TargetPointAnimation = new PointAnimation();
                Storyboard.SetTarget(_TargetPointAnimation, TargetTracker);
                Storyboard.SetTargetProperty(_TargetPointAnimation, "Position");
                _TargetPointAnimation.EasingFunction = new ElasticEase() { EasingMode = EasingMode.EaseOut };
                _TargetPointAnimation.EnableDependentAnimation = true;
                _TargetPointAnimation.Duration = TimeSpan.FromSeconds(0.1d);
                _TargetPointAnimation.To = new Point(0, 0);

                TargetMoveComplateAnimation = new Storyboard();
                TargetMoveComplateAnimation.Children.Add(_TargetPointAnimation);
                TargetMoveComplateAnimation.Completed += TargetMoveComplateAnimation_Completed;
            }
            if (SourceTracker != null)
            {
                SourceTracker.PositionChanged += SourceTracker_PositionChanged;

                _SourcePointAnimation = new PointAnimation();
                Storyboard.SetTarget(_SourcePointAnimation, SourceTracker);
                Storyboard.SetTargetProperty(_SourcePointAnimation, "Position");
                _SourcePointAnimation.EnableDependentAnimation = true;
                _SourcePointAnimation.Duration = TimeSpan.FromSeconds(0.05d);
                _SourcePointAnimation.From = new Point(0, 0);

                SourceMoveComplateAnimation = new Storyboard();
                SourceMoveComplateAnimation.Children.Add(_SourcePointAnimation);
                SourceMoveComplateAnimation.Completed += SourceMoveComplateAnimation_Completed;
            }
        }


        private void UpdatePosition()
        {
            if (Bezier1 != null && Bezier2 != null && Line1 != null)
            {
                var px = (TargetTracker.Position.X - SourceTracker.Position.X);
                var py = (TargetTracker.Position.Y - SourceTracker.Position.Y);

                //距原点距离
                var lenToO = Math.Sqrt(TargetTracker.Position.X * TargetTracker.Position.X + TargetTracker.Position.Y * TargetTracker.Position.Y);

                //两点间距离
                var len = Math.Sqrt(px * px + py * py);

                //两点间连线与X轴正半轴夹角
                var sina = py / len;
                var cosa = px / len;

                var baseCircleRadius = (Math.Abs(ThresholdRadius - len) / ThresholdRadius * 10 + 5) / 2;

                if (lenToO > ThresholdRadius)
                {
                    IsOverflow = true;
                    if (IsDragging)
                    {
                        VisualStateManager.GoToState(this, "Overflow", true);
                        _SourcePointAnimation.To = TargetTracker.Position;
                        SourceMoveComplateAnimation.Begin();
                        IsDragging = false;
                        IsOverflowAnimating = true;
                    }
                }
                else
                {
                    IsOverflow = false;
                }

                //x1 x2为锚点的两侧 x3为中点 x4 x5为目标的两侧
                //+5是移动到中心

                if (IsDragging || IsOverflowAnimating)
                {
                    var x1 = sina * baseCircleRadius + SourceTracker.Position.X + 7.5;
                    var y1 = -cosa * baseCircleRadius + SourceTracker.Position.Y + 7.5;

                    var x2 = -sina * baseCircleRadius + SourceTracker.Position.X + 7.5;
                    var y2 = cosa * baseCircleRadius + SourceTracker.Position.Y + 7.5;

                    var x3 = (TargetTracker.Position.X + SourceTracker.Position.X) / 2 + 7.5;
                    var y3 = (TargetTracker.Position.Y + SourceTracker.Position.Y) / 2 + 7.5;

                    var target_radius = Math.Min(PopupContentGrid.ActualWidth, PopupContentGrid.ActualHeight) / 2;

                    var x4 = sina * target_radius + TargetTracker.Position.X + 7.5;
                    var y4 = -cosa * target_radius + TargetTracker.Position.Y + 7.5;

                    var x5 = -sina * target_radius + TargetTracker.Position.X + 7.5;
                    var y5 = cosa * target_radius + TargetTracker.Position.Y + 7.5;

                    if (BaseCircleTrans != null)
                    {
                        var scale = baseCircleRadius / 7.5;
                        BaseCircleTrans.ScaleX = scale;
                        BaseCircleTrans.ScaleY = scale;
                    }

                    SetBezier1(new Point(x1, y1), new Point(x3, y3), new Point(x4, y4));
                    SetBezier2(new Point(x5, y5), new Point(x3, y3), new Point(x2, y2));
                }

                if (ContentGridTrans != null)
                {
                    ContentGridTrans.X = TargetTracker.Position.X;
                    ContentGridTrans.Y = TargetTracker.Position.Y;
                }
            }
        }

        /// <summary>
        /// Point1靠近起点 Point2在中间 Point3靠近终点
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <param name="Point3"></param>
        private void SetBezier1(Point Point1, Point Point2, Point Point3)
        {
            if (PathFigure1 != null)
            {
                PathFigure1.StartPoint = Point1;
            }
            if (Bezier1 != null)
            {
                Bezier1.Point1 = Point2;
                Bezier1.Point2 = Point3;
            }
        }

        /// <summary>
        /// Point1靠近起点 Point2在中间 Point3靠近终点
        /// </summary>
        /// <param name="Point1"></param>
        /// <param name="Point2"></param>
        /// <param name="Point3"></param>
        private void SetBezier2(Point Point1, Point Point2, Point Point3)
        {
            if (Line1 != null)
            {
                Line1.Point = Point1;
            }
            if (Bezier2 != null)
            {
                Bezier2.Point1 = Point2;
                Bezier2.Point2 = Point3;
            }
        }

        private void Judge()
        {
            if (TargetTracker != null)
            {
                if (IsOverflow)
                {
                    VisualStateManager.GoToState(this, "Normal", true);
                    this.Visibility = Visibility.Collapsed;
                    OnDragCompleted();
                }
                else
                {
                    if (TargetTracker.Position.X != 0 || TargetTracker.Position.Y != 0)
                    {
                        _TargetPointAnimation.From = TargetTracker.Position;
                        TargetMoveComplateAnimation.Begin();
                    }
                    VisualStateManager.GoToState(this, "Normal", true);
                }
            }

        }

        public double ThresholdRadius
        {
            get { return (double)GetValue(ThresholdRadiusProperty); }
            set { SetValue(ThresholdRadiusProperty, value); }
        }

        public static readonly DependencyProperty ThresholdRadiusProperty =
            DependencyProperty.Register("ThresholdRadius", typeof(double), typeof(DraggedBadge), new PropertyMetadata(100d, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is DraggedBadge sender)
                    {
                        if ((double)a.NewValue <= 0)
                        {
                            throw new ArgumentOutOfRangeException("ThresholdRadius 必须大于0");
                        }
                    }
                }
            }));


        public event DragCompletedEventHandler DragCompleted;
        private void OnDragCompleted()
        {
            DragCompleted.Invoke(this, new DragCompletedEventArgs());
        }
    }

    public delegate void DragCompletedEventHandler(object sender, DragCompletedEventArgs args);

    public class DragCompletedEventArgs : EventArgs
    {
        internal DragCompletedEventArgs() { }
    }
}
