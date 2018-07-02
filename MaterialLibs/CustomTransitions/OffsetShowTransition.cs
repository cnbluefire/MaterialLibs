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
    public class OffsetShowTransition : ShowTransitionBase
    {
        public OffsetShowTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        Vector3KeyFrameAnimation offset;
        ScalarKeyFrameAnimation opacity;

        protected override void OnConnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);
            var group = host.Compositor.CreateAnimationGroup();

            offset = host.Compositor.CreateVector3KeyFrameAnimation();
            offset.InsertExpressionKeyFrame(0f, "Vector3(this.StartingValue.X + offsetx,this.StartingValue.Y + offsety,this.StartingValue.Z)");
            offset.InsertExpressionKeyFrame(1f, "this.FinalValue");
            offset.SetReferenceParameter("host", host);
            offset.SetScalarParameter("offsetx", (float)OffsetX);
            offset.SetScalarParameter("offsety", (float)OffsetY);
            offset.Duration = Duration;
            offset.Target = "Offset";

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertKeyFrame(0f, 0f);
            opacity.InsertExpressionKeyFrame(1f, "this.FinalValue");
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
            DependencyProperty.Register("OffsetX", typeof(double), typeof(OffsetShowTransition), new PropertyMetadata(0d, OffsetXPropertyChanged));
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(OffsetShowTransition), new PropertyMetadata(30d, OffsetYPropertyChanged));

        private static void OffsetXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is OffsetShowTransition sender)
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
                if (d is OffsetShowTransition sender)
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
