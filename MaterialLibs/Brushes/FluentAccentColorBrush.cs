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
    public class FluentAccentColorBrush : XamlCompositionBrushBase
    {
        ColorKeyFrameAnimation ColorAnimation;
        ImplicitAnimationCollection ColorImplicitAnimations;

        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {
                CompositionBrush = Window.Current.Compositor.CreateColorBrush(Color);

                ColorAnimation = Window.Current.Compositor.CreateColorKeyFrameAnimation();
                ColorAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
                ColorAnimation.Duration = Duration;
                ColorAnimation.Target = "Color";

                ColorImplicitAnimations = Window.Current.Compositor.CreateImplicitAnimationCollection();
                ColorImplicitAnimations["Color"] = ColorAnimation;

                CompositionBrush.ImplicitAnimations = ColorImplicitAnimations;
            }
        }

        protected override void OnDisconnected()
        {
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
                ColorImplicitAnimations.Dispose();
                ColorImplicitAnimations = null;
                ColorAnimation.Dispose();
                ColorAnimation = null;
            }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(FluentAccentColorBrush), new PropertyMetadata(default(Color), (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentAccentColorBrush sender && sender.CompositionBrush is CompositionColorBrush colorBrush)
                    {
                        if (a.NewValue is Color color)
                        {
                            colorBrush.Color = color;
                        }
                    }
                }
            }));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(FluentAccentColorBrush), new PropertyMetadata(TimeSpan.FromSeconds(0.6d), (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentAccentColorBrush sender && sender.ColorAnimation != null)
                    {
                        sender.ColorAnimation.Duration = (TimeSpan)a.NewValue;
                    }
                }
            }));


    }
}
