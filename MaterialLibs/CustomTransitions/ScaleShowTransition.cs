using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.CustomTransitions
{
    public class ScaleShowTransition : ShowTransitionBase
    {
        public ScaleShowTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        Vector3KeyFrameAnimation scale;
        ScalarKeyFrameAnimation opacity;

        protected override void OnConnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);
            var group = host.Compositor.CreateAnimationGroup();

            var step = host.Compositor.CreateStepEasingFunction();

            scale = host.Compositor.CreateVector3KeyFrameAnimation();
            scale.InsertExpressionKeyFrame(0f, "Vector3(scalex,scaley,1f)");
            scale.InsertExpressionKeyFrame(1f, "this.FinalValue");
            scale.SetReferenceParameter("host", host);
            scale.SetScalarParameter("scalex", (float)ScaleX);
            scale.SetScalarParameter("scaley", (float)ScaleY);
            scale.Duration = Duration;
            scale.Target = "Scale";

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertKeyFrame(0f, 0f);
            opacity.InsertExpressionKeyFrame(1f, "this.FinalValue");
            opacity.Duration = Duration;
            opacity.Target = "Opacity";

            group.Add(scale);
            group.Add(opacity);

            Animation = group;
        }
        
        public double ScaleX
        {
            get { return (double)GetValue(ScaleXProperty); }
            set { SetValue(ScaleXProperty, value); }
        }

        public double ScaleY
        {
            get { return (double)GetValue(ScaleYProperty); }
            set { SetValue(ScaleYProperty, value); }
        }

        public static readonly DependencyProperty ScaleXProperty =
            DependencyProperty.Register("ScaleX", typeof(double), typeof(ScaleShowTransition), new PropertyMetadata(1.2, ScaleXPropertyChanged));
        public static readonly DependencyProperty ScaleYProperty =
            DependencyProperty.Register("ScaleY", typeof(double), typeof(ScaleShowTransition), new PropertyMetadata(1.2,ScaleYPropertyChanged));


        private static void ScaleXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScaleShowTransition sender)
                {
                    if (sender.scale != null)
                    {
                        sender.scale.SetScalarParameter("scalex", (float)e.NewValue);
                    }
                }
            }
        }

        private static void ScaleYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScaleShowTransition sender)
                {
                    if (sender.scale != null)
                    {
                        sender.scale.SetScalarParameter("scaley", (float)e.NewValue);
                    }
                }
            }
        }

        private void DurationPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (scale != null)
            {
                scale.Duration = Duration;
            }
            if (opacity != null)
            {
                opacity.Duration = Duration;
            }
        }
    }
}
