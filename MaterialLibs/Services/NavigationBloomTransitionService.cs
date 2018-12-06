using MaterialLibs.Common;
using MaterialLibs.Helpers;
using MaterialLibs.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;

namespace MaterialLibs.Services
{
    public class NavigationBloomTransitionService : INavigationBloomTransitionService
    {
        public NavigationBloomTransitionService()
        {
            drawer = new SurfaceDrawer(Compositor, new Size(1000, 1000));

            childVisual = Compositor.CreateSpriteVisual();
            childVisual.Size = new Vector2(1000f, 1000f);
            childVisual.Scale = Vector3.Zero;
            childVisual.Opacity = 0;
            childVisual.Brush = Compositor.CreateSurfaceBrush(drawer.Surface);
            childVisual.CenterPoint = new Vector3(500f, 500f, 0f);

            propSet = Compositor.CreatePropertySet();
            propSet.InsertScalar("scale", 0f);

            ScaleAnimation = Compositor.CreateVector3KeyFrameAnimation();
            ScaleAnimation.InsertKeyFrame(0f, Vector3.Zero);
            ScaleAnimation.InsertExpressionKeyFrame(1f, "Vector3(prop.scale, prop.scale, 1f)");
            ScaleAnimation.SetReferenceParameter("prop", propSet);
            ScaleAnimation.Duration = Duration;

            OpacityAnimation = Compositor.CreateScalarKeyFrameAnimation();
            OpacityAnimation.InsertKeyFrame(0f, 1f);
            OpacityAnimation.InsertKeyFrame(1f, 0f);
            OpacityAnimation.Duration = Duration;
        }

        private Compositor Compositor => Window.Current.Compositor;

        private SurfaceDrawer drawer;
        private SpriteVisual childVisual;
        private CompositionPropertySet propSet;
        private Vector3KeyFrameAnimation ScaleAnimation;
        private ScalarKeyFrameAnimation OpacityAnimation;

        private Color? _color = null;

        private Frame _CurrentFrame;
        private TimeSpan _Duration = TimeSpan.FromSeconds(0.5d);

        public NavigationState State { get; private set; }

        public Frame CurrentFrame
        {
            get => _CurrentFrame;
            set
            {
                if (_CurrentFrame != null)
                {
                    _CurrentFrame.Navigated -= _CurrentFrame_Navigated;
                }
                _CurrentFrame = value;
                if (_CurrentFrame != null)
                {
                    _CurrentFrame.Navigated += _CurrentFrame_Navigated;
                }
            }
        }

        public TimeSpan Duration
        {
            get => _Duration;
            set
            {
                _Duration = value;
                ScaleAnimation.Duration = Duration;
                OpacityAnimation.Duration = Duration;
            }
        }

        private float CalcRadius(Point Point)
        {
            var maxX = Math.Max(Point.X, CurrentFrame.ActualWidth - Point.X);
            var maxY = Math.Max(Point.Y, CurrentFrame.ActualHeight - Point.Y);
            return (float)Math.Sqrt(maxX * maxX + maxY * maxY);
        }

        private void Bloom(Point Point, Color Color)
        {
            if (!_color.HasValue || _color.Value != Color)
            {
                drawer.Draw((surface, session) =>
                {
                    session.Clear(Colors.Transparent);
                    session.FillCircle(new Vector2(500f, 500f), 500f, Color);
                    session.Flush();
                });
            }

            childVisual.StopAnimation("Scale");
            childVisual.StopAnimation("Opacity");
            childVisual.Scale = Vector3.Zero;
            childVisual.Opacity = 1;

            childVisual.Offset = new Vector3(Point.ToVector2() - new Vector2(500f, 500f), 0f);
            propSet.InsertScalar("scale", CalcRadius(Point) / 500f);
            ElementCompositionPreview.SetElementChildVisual(CurrentFrame, childVisual);

            childVisual.StartAnimation("Scale", ScaleAnimation);
        }

        public Task<bool> NavigateAndBloomFromPositionAsync(Point Point, Color color, Type sourcePageType, object parameter)
        {
            var completion = new TaskCompletionSource<bool>(false);
            if (State != NavigationState.None)
            {
                ElementCompositionPreview.SetElementChildVisual(CurrentFrame, null);
                childVisual.StopAnimation("Scale");
                childVisual.StopAnimation("Opacity");
                childVisual.Scale = Vector3.Zero;
                childVisual.Opacity = 0;
            }

            State = NavigationState.NavigatingAnimating;

            var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += (s, a) =>
            {
                if (State == NavigationState.NavigatingAnimating)
                {
                    State = NavigationState.NavigatingAnimated;
                }
                completion.SetResult(CurrentFrame.Navigate(sourcePageType, parameter, new SuppressNavigationTransitionInfo()));
            };
            Bloom(Point, color);
            batch.End();

            return completion.Task;
        }

        public Task<bool> NavigateAndBloomFromPositionAsync(Point Point, Color color, Type sourcePageType)
        {
            return NavigateAndBloomFromPositionAsync(Point, color, sourcePageType, null);
        }

        public Task<bool> NavigateAndBloomFromUIElementAsync(UIElement element, Color color, Type sourcePageType)
        {
            var ele = element as FrameworkElement;
            var point = element.TransformToVisual(CurrentFrame).TransformPoint(new Point(ele.ActualWidth / 2, ele.ActualHeight / 2));
            return NavigateAndBloomFromPositionAsync(point, color, sourcePageType, null);
        }

        public Task<bool> NavigateAndBloomFromUIElementAsync(UIElement element, Color color, Type sourcePageType, object parameter)
        {
            var ele = element as FrameworkElement;
            var point = element.TransformToVisual(CurrentFrame).TransformPoint(new Point(ele.ActualWidth / 2, ele.ActualHeight / 2));
            return NavigateAndBloomFromPositionAsync(point, color, sourcePageType, parameter);

        }

        private void _CurrentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (State == NavigationState.NavigatingAnimated)
            {
                State = NavigationState.NavigatedAnimating;
                var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, a) =>
                {
                    if (State == NavigationState.NavigatedAnimating)
                    {
                        childVisual.Scale = Vector3.Zero;
                        childVisual.Opacity = 0;
                        ElementCompositionPreview.SetElementChildVisual(CurrentFrame, null);
                        State = NavigationState.None;
                    }
                };
                childVisual.StartAnimation("Opacity", OpacityAnimation);
                batch.End();
            }
            else
            {
                ElementCompositionPreview.SetElementChildVisual(CurrentFrame, null);
                childVisual.StopAnimation("Scale");
                childVisual.StopAnimation("Opacity");
                childVisual.Scale = Vector3.Zero;
                childVisual.Opacity = 0;
            }
        }

    }

    public enum NavigationState
    {
        None,
        NavigatingAnimating,
        NavigatingAnimated,
        NavigatedAnimating,
    }
}
