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
    public class OffsetHideTransition : HideTransitionBase
    {
        public OffsetHideTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        Vector3KeyFrameAnimation offset;
        ScalarKeyFrameAnimation opacity;

        protected override void OnConnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);
            var group = host.Compositor.CreateAnimationGroup();

            var step = host.Compositor.CreateStepEasingFunction();

            var offset = host.Compositor.CreateVector3KeyFrameAnimation();
            offset.InsertExpressionKeyFrame(0f, "this.StartingValue");
            offset.InsertExpressionKeyFrame(0.99f, "Vector3(this.FinalValue.X + offsetx,this.FinalValue.Y + offsety,this.FinalValue.Z)");
            offset.InsertExpressionKeyFrame(1f, "this.FinalValue",step);
            offset.SetReferenceParameter("host", host);
            offset.SetScalarParameter("offsetx", (float)OffsetX);
            offset.SetScalarParameter("offsety", (float)OffsetY);
            offset.Duration = Duration;
            offset.Target = "Offset";

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertExpressionKeyFrame(0f, "this.FinalValue");
            opacity.InsertKeyFrame(0.99f, 0f);
            opacity.InsertKeyFrame(1f, 0f,step);
            opacity.Duration = Duration;
            opacity.Target = "Opacity";

            group.Add(offset);
            group.Add(opacity);

            Animation = group;
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(OffsetHideTransition), new PropertyMetadata(0d, OffsetXPropertyChanged));
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(OffsetHideTransition), new PropertyMetadata(30d, OffsetYPropertyChanged));

        private static void OffsetXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is OffsetHideTransition sender)
                {
                    if (sender.offset != null)
                    {
                        sender.offset.SetScalarParameter("offsetx", (float)e.NewValue);
                    }
                }
            }
        }

        private static void OffsetYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is OffsetHideTransition sender)
                {
                    if (sender.offset != null)
                    {
                        sender.offset.SetScalarParameter("offsety", (float)e.NewValue);
                    }
                }
            }
        }

        private void DurationPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (offset != null)
            {
                offset.Duration = Duration;
            }
            if (opacity != null)
            {
                opacity.Duration = Duration;
            }
        }

    }
}
