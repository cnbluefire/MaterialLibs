using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Brushes
{
    public class FluentSolidColorBrush : XamlCompositionBrushBase,IFluentBrush
    {
        Compositor Compositor => Window.Current.Compositor;
        ColorKeyFrameAnimation ColorAnimation;
        bool IsConnected;

        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {
                IsConnected = true;
                ColorAnimation = Compositor.CreateColorKeyFrameAnimation();
                ColorAnimation.InsertExpressionKeyFrame(0f, "this.StartingValue");
                ColorAnimation.InsertExpressionKeyFrame(1f, "Color");
                ColorAnimation.Duration = Duration;
                if (BaseBrush is SolidColorBrush colorBrush)
                {
                    CompositionBrush = Compositor.CreateColorBrush(colorBrush.Color);
                }
                else
                {
                    CompositionBrush = Compositor.CreateColorBrush();
                }
            }
        }

        protected override void OnDisconnected()
        {
            if(CompositionBrush!= null)
            {
                IsConnected = false;
                TransitionCompleted = null;

                ColorAnimation.Dispose();
                ColorAnimation = null;

                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }


        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(FluentSolidColorBrush), new PropertyMetadata(TimeSpan.FromSeconds(0.4d), (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentSolidColorBrush sender)
                    {
                        if (sender.ColorAnimation != null)
                        {
                            sender.ColorAnimation.Duration = (TimeSpan)a.NewValue;
                        }
                    }
                }
            }));


        public Brush BaseBrush
        {
            get { return (Brush)GetValue(BaseBrushProperty); }
            set { SetValue(BaseBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BaseBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BaseBrushProperty =
            DependencyProperty.Register("BaseBrush", typeof(Brush), typeof(FluentSolidColorBrush), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentSolidColorBrush sender)
                    {
                        if (sender.CompositionBrush != null && sender.ColorAnimation != null)
                        {
                            if (a.NewValue is SolidColorBrush brush)
                            {
                                sender.ColorAnimation.SetColorParameter("Color", brush.Color);
                                var batch = sender.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                                batch.Completed += (s1, a1) =>
                                {
                                    var isConnected = sender.IsConnected;
                                    if (isConnected)
                                    {
                                        sender.OnTransitionCompleted(a.OldValue as SolidColorBrush, brush);
                                    }
                                };
                                sender.CompositionBrush.StartAnimation("Color", sender.ColorAnimation);
                                batch.End();
                                batch.Dispose();
                            }
                        }
                    }
                }
            }));


        public event TransitionCompletedEventHandler TransitionCompleted;
        private void OnTransitionCompleted(Brush oldBrush, Brush newBrush)
        {
            TransitionCompleted?.Invoke(this, new TransitionCompletedEventArgs()
            {
                OldBrush = oldBrush,
                NewBrush = newBrush
            });
        }
    }

}
