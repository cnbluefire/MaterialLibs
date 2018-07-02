using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.CustomTransitions
{
    public class FlipShowTransition : ShowTransitionBase
    {
        public FlipShowTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        CompositionPropertySet propset;
        Vector3KeyFrameAnimation offset;
        Vector3KeyFrameAnimation axis;
        ScalarKeyFrameAnimation degress;
        ScalarKeyFrameAnimation opacity;
        Visual host;

        protected override void OnConnected(UIElement element)
        {
            host = ElementCompositionPreview.GetElementVisual(element);
            propset = host.Compositor.CreatePropertySet();
            propset.InsertScalar("offsetx", 0f);
            propset.InsertScalar("offsety", 0f);
            propset.InsertScalar("degress", 0f);
            propset.InsertVector3("axis", Vector3.UnitZ);

            UpdateFrom();

            var group = host.Compositor.CreateAnimationGroup();

            var step = host.Compositor.CreateStepEasingFunction();

            offset = host.Compositor.CreateVector3KeyFrameAnimation();
            offset.InsertExpressionKeyFrame(0f, "Vector3(this.StartingValue.X + prop.offsetx,this.StartingValue.Y + prop.offsety,this.StartingValue.Z)");
            offset.InsertExpressionKeyFrame(1f, "this.FinalValue");
            offset.SetReferenceParameter("host", host);
            offset.SetReferenceParameter("prop", propset);
            offset.Duration = Duration;
            offset.Target = "Offset";

            axis = host.Compositor.CreateVector3KeyFrameAnimation();
            axis.InsertExpressionKeyFrame(0f, "prop.axis",step);
            axis.InsertExpressionKeyFrame(1f, "prop.axis", step);
            axis.SetReferenceParameter("prop", propset);
            axis.Duration = Duration;
            axis.Target = "RotationAxis";

            degress = host.Compositor.CreateScalarKeyFrameAnimation();
            degress.InsertExpressionKeyFrame(0f, "this.StartingValue + prop.degress");
            degress.InsertExpressionKeyFrame(1f, "this.FinalValue");
            degress.SetReferenceParameter("host", host);
            degress.SetReferenceParameter("prop", propset);
            degress.Duration = Duration;
            degress.Target = "RotationAngleInDegrees";

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertKeyFrame(0f, 0f);
            opacity.InsertExpressionKeyFrame(1f, "this.FinalValue");
            opacity.Duration = Duration;
            opacity.Target = "Opacity";

            group.Add(offset);
            group.Add(axis);
            group.Add(degress);
            group.Add(opacity);

            Animation = group;
        }

        protected override void OnDisconnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);
            host.RotationAxis = new Vector3(0f, 0f, 1f);
        }

        private void UpdateFrom()
        {
            if (host == null) return;
            propset.StopAnimation("offsetx");
            propset.StopAnimation("offsety");
            ExpressionAnimation an = null;
            switch (From)
            {
                case FlipTransitionMode.Left:
                    an = host.Compositor.CreateExpressionAnimation("-host.Size.X");
                    an.SetReferenceParameter("host", host);
                    propset.StartAnimation("offsetx", an);
                    propset.InsertScalar("offsety", 0f);
                    propset.InsertScalar("degress", 90f);
                    propset.InsertVector3("axis", Vector3.UnitY);
                    break;
                case FlipTransitionMode.Top:
                    an = host.Compositor.CreateExpressionAnimation("-host.Size.Y");
                    an.SetReferenceParameter("host", host);
                    propset.InsertScalar("offsetx", 0f);
                    propset.StartAnimation("offsety", an);
                    propset.InsertScalar("degress", 90f);
                    propset.InsertVector3("axis", Vector3.UnitX);
                    break;
                case FlipTransitionMode.Right:
                    an = host.Compositor.CreateExpressionAnimation("host.Size.X");
                    an.SetReferenceParameter("host", host);
                    propset.StartAnimation("offsetx", an);
                    propset.InsertScalar("offsety", 0f);
                    propset.InsertScalar("degress", -90f);
                    propset.InsertVector3("axis", Vector3.UnitY);
                    break;
                case FlipTransitionMode.Bottom:
                    an = host.Compositor.CreateExpressionAnimation("host.Size.Y");
                    an.SetReferenceParameter("host", host);
                    propset.InsertScalar("offsetx", 0f);
                    propset.StartAnimation("offsety", an);
                    propset.InsertScalar("degress", -90f);
                    propset.InsertVector3("axis", Vector3.UnitX);
                    break;
            }
        }

        public FlipTransitionMode From
        {
            get { return (FlipTransitionMode)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(FlipTransitionMode), typeof(FlipShowTransition), new PropertyMetadata(FlipTransitionMode.Bottom, FromPropertyChanged));

        private static void FromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((FlipTransitionMode)e.NewValue != (FlipTransitionMode)e.OldValue)
            {
                if(d is FlipShowTransition sender)
                {
                    sender.UpdateFrom();
                }
            }
        }

        private void DurationPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (offset != null)
            {
                offset.Duration = Duration;
            }
            if (degress != null)
            {
                degress.Duration = Duration;
            }
            if (opacity != null)
            {
                opacity.Duration = Duration;
            }
        }

    }
}
