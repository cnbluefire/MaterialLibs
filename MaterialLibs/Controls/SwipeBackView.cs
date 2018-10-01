using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class SwipeBackView : ContentControl
    {
        public SwipeBackView()
        {
            this.DefaultStyleKey = typeof(SwipeBackView);
            this.SizeChanged += SwipeBackView_SizeChanged;
        }

        Grid ContentGrid;
        TranslateTransform ContentGridTrans;
        Rectangle GestureRect;
        Rectangle LightDismissLayer;
        Rectangle ShadowHost;
        RectangleGeometry RootClip;

        Storyboard CompletedStoryboard;
        DoubleAnimationUsingKeyFrames CompletedAnimation;
        SplineDoubleKeyFrame CompletedKey1;
        SplineDoubleKeyFrame CompletedKey2;

        Compositor Compositor => Window.Current.Compositor;
        SpriteVisual ShadowVisual;
        DropShadow Shadow;
        bool IsLoaded = false;
        bool IsCoreCompleteChanged = false;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ContentGrid = GetTemplateChild("ContentGrid") as Grid;
            ContentGridTrans = GetTemplateChild("ContentGridTrans") as TranslateTransform;
            GestureRect = GetTemplateChild("GestureRect") as Rectangle;
            LightDismissLayer = GetTemplateChild("LightDismissLayer") as Rectangle;
            ShadowHost = GetTemplateChild("ShadowHost") as Rectangle;
            RootClip = GetTemplateChild("RootClip") as RectangleGeometry;

            if (ContentGrid != null)
            {
                ContentGrid.SizeChanged += ContentGrid_SizeChanged;

                ContentGrid.Width = this.ActualWidth;
                ContentGrid.Height = this.ActualHeight;

                if (ShadowHost != null)
                {
                    Shadow = Compositor.CreateDropShadow();
                    Shadow.Color = Colors.Black;
                    Shadow.BlurRadius = 16f;
                    Shadow.Opacity = 0.8f;

                    ShadowVisual = Compositor.CreateSpriteVisual();
                    ShadowVisual.Size = new Vector2((float)ContentGrid.ActualWidth, (float)ContentGrid.ActualHeight);
                    ShadowVisual.Shadow = Shadow;
                    ShadowVisual.IsVisible = false;

                    ElementCompositionPreview.SetElementChildVisual(ShadowHost, ShadowVisual);
                }
            }

            if (GestureRect != null)
            {
                GestureRect.ManipulationStarted += GestureRect_ManipulationStarted;
                GestureRect.ManipulationDelta += GestureRect_ManipulationDelta;
                GestureRect.ManipulationCompleted += GestureRect_ManipulationCompleted;
            }

            if(LightDismissLayer != null)
            {
                LightDismissLayer.Tapped += LightDismissLayer_Tapped;
            }

            if (RootClip != null)
            {
                RootClip.Rect = new Windows.Foundation.Rect(0, 0, this.ActualWidth, this.ActualHeight);
            }

            CompletedKey1 = new SplineDoubleKeyFrame()
            {
                Value = 0,
                KeySpline = new KeySpline()
                {
                    ControlPoint1 = new Windows.Foundation.Point(0.1, 0.9),
                    ControlPoint2 = new Windows.Foundation.Point(0.2, 1.0),
                },
                KeyTime = TimeSpan.Zero,
            };

            CompletedKey2 = new SplineDoubleKeyFrame()
            {
                Value = 0,
                KeySpline = new KeySpline()
                {
                    ControlPoint1 = new Windows.Foundation.Point(0.1, 0.9),
                    ControlPoint2 = new Windows.Foundation.Point(0.2, 1.0),
                },
                KeyTime = TimeSpan.FromSeconds(0.4d),
            };

            CompletedAnimation = new DoubleAnimationUsingKeyFrames()
            {
                Duration = TimeSpan.FromSeconds(0.4d),
            };
            CompletedAnimation.KeyFrames.Add(CompletedKey1);
            CompletedAnimation.KeyFrames.Add(CompletedKey2);

            Storyboard.SetTarget(CompletedAnimation, ContentGridTrans);
            Storyboard.SetTargetProperty(CompletedAnimation, "X");

            CompletedStoryboard = new Storyboard();
            CompletedStoryboard.Children.Add(CompletedAnimation);
            CompletedStoryboard.Completed += CompletedStoryboard_Completed;

            UpdateIsOpen(false);

            IsLoaded = true;
        }

        private void UpdateIsOpen(bool IsAnimationEnable, double? delta = null)
        {
            if (ContentGrid == null) return;
            if (IsOpen)
            {
                if (delta.HasValue)
                {
                    CompletedKey1.Value = Math.Max(0, delta.Value);
                }
                else
                {
                    CompletedKey1.Value = ContentGrid.ActualWidth;
                }
                CompletedKey2.Value = 0;
                VisualStateManager.GoToState(this, "Normal", IsAnimationEnable);
            }
            else
            {
                if (delta.HasValue)
                {
                    CompletedKey1.Value = Math.Max(0, delta.Value);
                }
                else
                {
                    CompletedKey1.Value = 0;
                }
                CompletedKey2.Value = ContentGrid.ActualWidth;
                VisualStateManager.GoToState(this, "Completed", IsAnimationEnable);
            }
            if (IsAnimationEnable)
            {
                if (ShadowVisual != null)
                {
                    ShadowVisual.IsVisible = true;
                }
                CompletedStoryboard.Begin();
            }
            else
            {
                CompletedStoryboard.SkipToFill();
            }
        }

        private void SwipeBackView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RootClip != null)
            {
                RootClip.Rect = new Windows.Foundation.Rect(0, 0, e.NewSize.Width, e.NewSize.Height);
            }
            if(ContentGrid != null)
            {
                ContentGrid.Width = e.NewSize.Width;
                ContentGrid.Height = e.NewSize.Height;
            }
        }

        private void ContentGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ShadowVisual != null)
            {
                ShadowVisual.Size = e.NewSize.ToVector2();
            }
        }

        private void GestureRect_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Swiping", true);
            if (ContentGridTrans != null)
            {
                ContentGridTrans.X = 0;
            }
            if (ShadowVisual != null)
            {
                ShadowVisual.IsVisible = true;
            }
        }

        private void GestureRect_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (ContentGridTrans != null && ContentGrid != null)
            {
                ContentGridTrans.X = Math.Min(Math.Max(e.Cumulative.Translation.X, 0), ContentGrid.ActualWidth);
                if (LightDismissLayer != null && ContentGrid.ActualWidth > 0)
                {
                    LightDismissLayer.Opacity = Math.Max(0, 1 - (ContentGridTrans.X / (ContentGrid.ActualWidth)));
                }
            }
        }

        private void GestureRect_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            double speed = e.Velocities.Linear.X;
            double delta = e.Cumulative.Translation.X;

            IsCoreCompleteChanged = true;
            if (ContentGrid != null && delta < ContentGrid.ActualWidth / 3 && speed < 0.5)
            {
                IsOpen = true;
            }
            else
            {
                IsOpen = false;
            }
            IsCoreCompleteChanged = false;
            UpdateIsOpen(true, delta);
        }

        private void LightDismissLayer_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (IsLightDismissEnabled)
            {
                IsOpen = false;
            }
        }

        private void CompletedStoryboard_Completed(object sender, object e)
        {
            if (ShadowVisual != null)
            {
                if (AlwaysShowShadow)
                {
                    if (!IsOpen)
                    {
                        ShadowVisual.IsVisible = false;
                    }
                }
                else
                {
                    ShadowVisual.IsVisible = false;
                }
            }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public Brush LightDismissBackground
        {
            get { return (Brush)GetValue(LightDismissBackgroundProperty); }
            set { SetValue(LightDismissBackgroundProperty, value); }
        }

        public bool IsSwipeEnable
        {
            get { return (bool)GetValue(IsSwipeEnableProperty); }
            set { SetValue(IsSwipeEnableProperty, value); }
        }

        public bool AlwaysShowShadow
        {
            get { return (bool)GetValue(AlwaysShowShadowProperty); }
            set { SetValue(AlwaysShowShadowProperty, value); }
        }

        public double ContentMaxWidth
        {
            get { return (double)GetValue(ContentMaxWidthProperty); }
            set { SetValue(ContentMaxWidthProperty, value); }
        }

        public double ContentMaxHeight
        {
            get { return (double)GetValue(ContentMaxHeightProperty); }
            set { SetValue(ContentMaxHeightProperty, value); }
        }

        public bool IsLightDismissEnabled
        {
            get { return (bool)GetValue(IsLightDismissEnabledProperty); }
            set { SetValue(IsLightDismissEnabledProperty, value); }
        }


        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(SwipeBackView), new PropertyMetadata(false, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is SwipeBackView sender)
                    {
                        if (sender.IsLoaded)
                        {
                            sender.OnIsOpenChanged();
                            if (sender.IsCoreCompleteChanged)
                            {
                                return;
                            }
                            sender.UpdateIsOpen(true);
                        }
                    }
                }
            }));

        public static readonly DependencyProperty LightDismissBackgroundProperty =
            DependencyProperty.Register("LightDismissBackground", typeof(Brush), typeof(SwipeBackView), new PropertyMetadata(null));

        public static readonly DependencyProperty IsSwipeEnableProperty =
            DependencyProperty.Register("IsSwipeEnable", typeof(bool), typeof(SwipeBackView), new PropertyMetadata(true));

        public static readonly DependencyProperty AlwaysShowShadowProperty =
            DependencyProperty.Register("AlwaysShowShadow", typeof(bool), typeof(SwipeBackView), new PropertyMetadata(false));

        public static readonly DependencyProperty ContentMaxWidthProperty =
            DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(SwipeBackView), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty ContentMaxHeightProperty =
            DependencyProperty.Register("ContentMaxHeight", typeof(double), typeof(SwipeBackView), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty IsLightDismissEnabledProperty =
            DependencyProperty.Register("IsLightDismissEnabled", typeof(bool), typeof(SwipeBackView), new PropertyMetadata(true));

        public event EventHandler IsOpenChanged;
        private void OnIsOpenChanged()
        {
            IsOpenChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
