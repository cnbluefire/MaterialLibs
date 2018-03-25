using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MaterialLibs
{
    public sealed partial class AnimationHamburgerIcon : UserControl
    {
        public AnimationHamburgerIcon()
        {
            this.InitializeComponent();
            propSet = compositor.CreatePropertySet();
            propSet.InsertScalar("progress", 0f);
            propSet.InsertScalar("isopened", 0f);
            propSet.InsertScalar("newvalue", 0f);
        }

        private Compositor compositor => Window.Current.Compositor;

        Visual TopBorderVisual;
        Visual BottomBorderVisual;
        Visual CenterBorderVisual;
        Visual ContentBorderVisual;

        CompositionPropertySet propSet;
        //ImplicitAnimationCollection ImplicitAnimations;

        ExpressionAnimation TopRotationAnimation;
        ExpressionAnimation BottomRotationAnimation;
        ExpressionAnimation IconRotationAnimation;
        ExpressionAnimation ExternalScaleXAnimation;
        ExpressionAnimation InternalScaleXAnimation;

        ScalarKeyFrameAnimation ProgressAnimation;
        ScalarKeyFrameAnimation IsOpenedAnimation;
        ScalarKeyFrameAnimation IsNotOpenedAnimation;

        CubicBezierEasingFunction easing;
        StepEasingFunction steping;

        void InitConpositionResources()
        {
            easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1f));
            steping = compositor.CreateStepEasingFunction(2);

            ProgressAnimation = compositor.CreateScalarKeyFrameAnimation();
            ProgressAnimation.InsertExpressionKeyFrame(0f, "This.StartingValue", easing);
            ProgressAnimation.InsertExpressionKeyFrame(1f, "propSet.newvalue", easing);
            ProgressAnimation.SetReferenceParameter("propSet", propSet);
            ProgressAnimation.Duration = TimeSpan.FromSeconds(0.3d);
            ProgressAnimation.Target = "progress";

            //ImplicitAnimations = compositor.CreateImplicitAnimationCollection();
            //ImplicitAnimations["progress"] = ProgressAnimation;

            //propSet.ImplicitAnimations = ImplicitAnimations;

            TopBorderVisual = ElementCompositionPreview.GetElementVisual(TopBorder);
            BottomBorderVisual = ElementCompositionPreview.GetElementVisual(BottomBorder);
            CenterBorderVisual = ElementCompositionPreview.GetElementVisual(CenterBorder);
            ContentBorderVisual = ElementCompositionPreview.GetElementVisual(ContentBorder);

            TopBorderVisual.CenterPoint = new Vector3(38f, 36f, 0f);
            BottomBorderVisual.CenterPoint = new Vector3(39f, 36f, 0f);
            CenterBorderVisual.CenterPoint = new Vector3(36f, 36f, 0f);
            ContentBorderVisual.CenterPoint = new Vector3(36, 36, 0);

            TopRotationAnimation = compositor.CreateExpressionAnimation("0.25 * Pi * propSet.progress");
            TopRotationAnimation.SetReferenceParameter("propSet", propSet);

            BottomRotationAnimation = compositor.CreateExpressionAnimation("-0.25 * Pi * propSet.progress");
            BottomRotationAnimation.SetReferenceParameter("propSet", propSet);

            IconRotationAnimation = compositor.CreateExpressionAnimation("propSet.isopened == 1 ? -Pi * propSet.progress : Pi * propSet.progress");
            IconRotationAnimation.SetReferenceParameter("propSet", propSet);

            ExternalScaleXAnimation = compositor.CreateExpressionAnimation(" 1 - 0.4 * propSet.progress");
            ExternalScaleXAnimation.SetReferenceParameter("propSet", propSet);

            InternalScaleXAnimation = compositor.CreateExpressionAnimation(" 1 - 0.04 * propSet.progress");
            InternalScaleXAnimation.SetReferenceParameter("propSet", propSet);

            IsOpenedAnimation = compositor.CreateScalarKeyFrameAnimation();
            IsOpenedAnimation.InsertExpressionKeyFrame(0f, "This.StartingValue", steping);
            IsOpenedAnimation.InsertKeyFrame(1f, 1f, steping);
            IsOpenedAnimation.Duration = TimeSpan.FromSeconds(0.001d);
            IsOpenedAnimation.DelayTime = TimeSpan.FromSeconds(0.3d);

            IsNotOpenedAnimation = compositor.CreateScalarKeyFrameAnimation();
            IsNotOpenedAnimation.InsertExpressionKeyFrame(0f, "This.StartingValue", steping);
            IsNotOpenedAnimation.InsertKeyFrame(1f, 0f, steping);
            IsNotOpenedAnimation.Duration = TimeSpan.FromSeconds(0.001d);
            IsNotOpenedAnimation.DelayTime = TimeSpan.FromSeconds(0.3d);

            TopBorderVisual.StartAnimation("RotationAngle", TopRotationAnimation);
            BottomBorderVisual.StartAnimation("RotationAngle", BottomRotationAnimation);
            ContentBorderVisual.StartAnimation("RotationAngle", IconRotationAnimation);

            TopBorderVisual.StartAnimation("Scale.X", ExternalScaleXAnimation);
            BottomBorderVisual.StartAnimation("Scale.X", ExternalScaleXAnimation);
            CenterBorderVisual.StartAnimation("Scale.X", InternalScaleXAnimation);
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(AnimationHamburgerIcon), new PropertyMetadata(0, ProgressPropertyChanged));

        private static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;
            var sender = d as AnimationHamburgerIcon;

            float progress = Convert.ToSingle(e.NewValue);
            float oldprogress = Convert.ToSingle(e.OldValue);
            if (progress > 1f || progress < 0f) throw new ArgumentException("Progress必须在0到1之间！");

            ProgressPropertyChanged(sender, progress, oldprogress);
        }

        private static void ProgressPropertyChanged(AnimationHamburgerIcon sender,float progress,float oldprogress)
        {
            if (sender.propSet == null) return;

            sender.propSet.StopAnimation("progress");
            if (Math.Abs(oldprogress - progress) < 0.05f)
            {
                sender.propSet.InsertScalar("progress", progress);
                if (sender.IsOpenedAnimation != null)
                {
                    if (progress == 1)
                    {
                        sender.propSet.StopAnimation("isopened");
                        sender.propSet.InsertScalar("isopened", 1f);
                    }
                    if (progress == 0)
                    {
                        sender.propSet.StopAnimation("isopened");
                        sender.propSet.InsertScalar("isopened", 0f);
                    }
                }
            }
            else
            {
                if (sender.ProgressAnimation == null) return;
                sender.propSet.InsertScalar("newvalue", progress);
                sender.propSet.StartAnimation("progress", sender.ProgressAnimation);
                if (sender.IsOpenedAnimation != null)
                {
                    if (progress == 1)
                    {
                        sender.propSet.StopAnimation("isopened");
                        sender.propSet.StartAnimation("isopened", sender.IsOpenedAnimation);
                    }
                    if (progress == 0)
                    {
                        sender.propSet.StopAnimation("isopened");
                        sender.propSet.StartAnimation("isopened", sender.IsNotOpenedAnimation);
                    }
                }
            }

            if (progress == 1)
            {
                sender.TopBorder.Visibility = Visibility.Collapsed;
                sender.CenterBorder.Visibility = Visibility.Collapsed;
                sender.BottomBorder.Visibility = Visibility.Collapsed;
                sender.BackBorder.Visibility = Visibility.Visible;
            }
            else
            {
                sender.TopBorder.Visibility = Visibility.Visible;
                sender.CenterBorder.Visibility = Visibility.Visible;
                sender.BottomBorder.Visibility = Visibility.Visible;
                sender.BackBorder.Visibility = Visibility.Collapsed;
            }
        }

        public bool IsEnded
        {
            get { return (bool)GetValue(IsEndedProperty); }
            set { SetValue(IsEndedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEnded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEndedProperty =
            DependencyProperty.Register("IsEnded", typeof(bool), typeof(AnimationHamburgerIcon), new PropertyMetadata(false, (s, a) =>
            {
                if (a.NewValue == a.OldValue) return;
                var value = (bool)a.NewValue;
                var sender = s as AnimationHamburgerIcon;
                if (value == false) sender.Progress = 0;
                else sender.Progress = 1;
            }));





        private void ContentBorder_Loaded(object sender, RoutedEventArgs e)
        {
            InitConpositionResources();
            if (IsEnded)
            {
                ProgressPropertyChanged(this, 1f, 0f);
            }
        }
    }
}
