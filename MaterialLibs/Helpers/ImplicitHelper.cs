using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Helpers
{
    public static class ImplicitAnimationHelper
    {
        private const string ScalarTargets = "Opacity,RotationAngleInDegrees,RotationAngle,BlurRadius";
        private const string Vector2Targets = "Size,AnchorPoint";
        private const string Vector3Targets = "CenterPoint,Offset,Scale,RotationAxis";
        private const string Vector4Targets = "";
        private const string QuaternionTargets = "Orientation";
        private const string ColorTargets = "Color";

        public static void CreateAnimation<T>(CompositionObject obj, string Target, TimeSpan Duration) where T : struct
        {
            var compositor = obj.Compositor;

            KeyFrameAnimation an = null;
            if (typeof(T) == typeof(float)) an = compositor.CreateScalarKeyFrameAnimation();
            if (typeof(T) == typeof(Vector2)) an = compositor.CreateVector2KeyFrameAnimation();
            if (typeof(T) == typeof(Vector3)) an = compositor.CreateVector3KeyFrameAnimation();
            if (typeof(T) == typeof(Vector4)) an = compositor.CreateVector4KeyFrameAnimation();
            if (typeof(T) == typeof(Quaternion)) an = compositor.CreateQuaternionKeyFrameAnimation();
            if (typeof(T) == typeof(Color)) an = compositor.CreateColorKeyFrameAnimation();
            if (an == null) return;
            an.InsertExpressionKeyFrame(1f, "this.FinalValue");
            an.Duration = Duration;
            an.Target = Target;
            if (obj.ImplicitAnimations == null)
            {
                obj.ImplicitAnimations = compositor.CreateImplicitAnimationCollection();
            }
            obj.ImplicitAnimations[Target] = an;
        }

        public static void CreateAnimation(CompositionObject obj, string Target, TimeSpan Duration)
        {
            var compositor = obj.Compositor;

            KeyFrameAnimation an = null;
            if (ScalarTargets.Contains(Target)) an = compositor.CreateScalarKeyFrameAnimation();
            if (Vector2Targets.Contains(Target)) an = compositor.CreateVector2KeyFrameAnimation();
            if (Vector3Targets.Contains(Target)) an = compositor.CreateVector3KeyFrameAnimation();
            if (Vector4Targets.Contains(Target)) an = compositor.CreateVector4KeyFrameAnimation();
            if (QuaternionTargets.Contains(Target)) an = compositor.CreateQuaternionKeyFrameAnimation();
            if (ColorTargets.Contains(Target)) an = compositor.CreateColorKeyFrameAnimation();
            if (an == null) return;
            an.InsertExpressionKeyFrame(1f, "this.FinalValue");
            an.Duration = Duration;
            an.Target = Target;
            if (obj.ImplicitAnimations == null)
            {
                obj.ImplicitAnimations = compositor.CreateImplicitAnimationCollection();
            }
            obj.ImplicitAnimations[Target] = an;
        }

        public static void RemoveAnimation(CompositionObject obj, string Target)
        {
            if (obj.ImplicitAnimations != null)
            {
                if (obj.ImplicitAnimations.ContainsKey(Target))
                {
                    obj.ImplicitAnimations.Remove(Target);
                }
            }
        }
    }

    public class ImplicitHelper
    {
        public static TimeSpan GetDuration(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(DurationProperty);
        }

        public static void SetDuration(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(DurationProperty, value);
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(ImplicitHelper), new PropertyMetadata(TimeSpan.FromSeconds(0.3d), DurationPropertyChanged));

        private static void DurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is TimeSpan time)
            {
                if (d is UIElement sender)
                {
                    var host = ElementCompositionPreview.GetElementVisual(sender);
                    if (host.ImplicitAnimations != null)
                    {
                        foreach (var item in host.ImplicitAnimations)
                        {
                            if (item.Value is KeyFrameAnimation an)
                            {
                                an.Duration = time;
                            }
                        }
                    }
                }
            }
        }

        public static string GetTargets(UIElement obj)
        {
            return (string)obj.GetValue(TargetsProperty);
        }

        public static void SetTargets(UIElement obj, string value)
        {
            obj.SetValue(TargetsProperty, value);
        }

        public static readonly DependencyProperty TargetsProperty =
            DependencyProperty.RegisterAttached("Targets", typeof(string), typeof(ImplicitHelper), new PropertyMetadata(null, TargetsPropertyChanged));

        private static void TargetsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string nstr)
            {
                if (d is UIElement sender)
                {
                    var host = ElementCompositionPreview.GetElementVisual(sender);
                    var narr = nstr.Replace(" ", "").Split(',');
                    var duration = GetDuration(sender);
                    host.ImplicitAnimations = null;
                    foreach (var target in narr)
                    {
                        ImplicitAnimationHelper.CreateAnimation(host, target, duration);
                    }
                }
            }
        }
    }
}
