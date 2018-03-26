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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Helpers
{
    public class RippleHelper : DependencyObject
    {
        #region Field
        private static readonly PointerEventHandler pointerEventHandler = new PointerEventHandler(Ele_PointerReleased);
        private static Compositor compositor => Window.Current.Compositor;
        private static ExpressionAnimation _SizeBind;
        private static CompositionEasingFunction _EaseOut;
        private static ScalarKeyFrameAnimation _OpacityAnimation;
        private static Vector3KeyFrameAnimation _ScaleAnimation;
        private static CompositionAnimationGroup _RippleAnimationGroup;
        private static CompositionPropertySet _PropSet;
        private static CompositionBrush _Mask;
        #endregion

        #region Property
        private static ExpressionAnimation SizeBind
        {
            get
            {
                if (_SizeBind == null) _SizeBind = compositor.CreateExpressionAnimation("hostVisual.Size");
                return _SizeBind;
            }
        }
        private static CompositionEasingFunction EaseOut
        {
            get
            {
                if (_EaseOut == null) _EaseOut = compositor.CreateCubicBezierEasingFunction(new Vector2(0f, 0f), new Vector2(0.9f, 1f));
                return _EaseOut;
            }
        }
        private static ScalarKeyFrameAnimation OpacityAnimation
        {
            get
            {
                if (_OpacityAnimation == null)
                {
                    _OpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
                    _OpacityAnimation.InsertKeyFrame(0f, 1f, EaseOut);
                    _OpacityAnimation.InsertKeyFrame(1f, 0f, EaseOut);
                    _OpacityAnimation.Duration = TimeSpan.FromMilliseconds(350);
                    _OpacityAnimation.Target = "Opacity";
                }
                return _OpacityAnimation;
            }
        }
        private static Vector3KeyFrameAnimation ScaleAnimation
        {
            get
            {
                if (_ScaleAnimation == null)
                {
                    _ScaleAnimation = compositor.CreateVector3KeyFrameAnimation();
                    _ScaleAnimation.InsertKeyFrame(0f, new Vector3(0f, 0f, 1f), EaseOut);
                    _ScaleAnimation.InsertExpressionKeyFrame(0.8f, "Vector3(propSet.ScaleValue,propSet.ScaleValue,1f)", EaseOut);
                    _ScaleAnimation.InsertExpressionKeyFrame(1f, "Vector3(propSet.ScaleValue,propSet.ScaleValue,1f)", EaseOut);
                    _ScaleAnimation.SetReferenceParameter("propSet", PropSet);
                    _ScaleAnimation.Duration = TimeSpan.FromMilliseconds(320);
                    _ScaleAnimation.Target = "Scale";
                }
                return _ScaleAnimation;
            }
        }
        private static CompositionAnimationGroup RippleAnimationGroup
        {
            get
            {
                if (_RippleAnimationGroup == null)
                {
                    _RippleAnimationGroup = compositor.CreateAnimationGroup();
                    _RippleAnimationGroup.Add(OpacityAnimation);
                    _RippleAnimationGroup.Add(ScaleAnimation);
                }
                return _RippleAnimationGroup;
            }
        }
        private static CompositionPropertySet PropSet
        {
            get
            {
                if (_PropSet == null)
                {
                    _PropSet = compositor.CreatePropertySet();
                    PropSet.InsertScalar("ScaleValue", 2f);
                }
                return _PropSet;
            }
        }
        private static CompositionBrush Mask
        {
            get
            {
                if (_Mask == null)
                {
                    var surface = LoadedImageSurface.StartLoadFromUri(new Uri("ms-appx:///MaterialLibs/Assets/RippleMask.png"), new Windows.Foundation.Size(100d, 100d));
                    _Mask = compositor.CreateSurfaceBrush(surface);
                }
                return _Mask;
            }
        }
        #endregion

        #region AttachedProperty

        public static bool GetIsFillEnable(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFillEnableProperty);
        }

        public static void SetIsFillEnable(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFillEnableProperty, value);
        }
        public static readonly DependencyProperty IsFillEnableProperty =
            DependencyProperty.RegisterAttached("IsFillEnable", typeof(bool), typeof(RippleHelper), new PropertyMetadata(false));


        public static TimeSpan GetRippleDuration(UIElement obj)
        {
            return (TimeSpan)obj.GetValue(RippleDurationProperty);
        }
        public static void SetRippleDuration(UIElement obj, TimeSpan value)
        {
            obj.SetValue(RippleDurationProperty, value);
        }
        public static readonly DependencyProperty RippleDurationProperty =
            DependencyProperty.RegisterAttached("RippleDuration", typeof(TimeSpan), typeof(RippleHelper), new PropertyMetadata(TimeSpan.FromMilliseconds(330)));


        public static double GetRippleRadius(UIElement obj)
        {
            return (double)obj.GetValue(RippleRadiusProperty);
        }

        public static void SetRippleRadius(UIElement obj, double value)
        {
            obj.SetValue(RippleRadiusProperty, value);
        }
        public static readonly DependencyProperty RippleRadiusProperty =
            DependencyProperty.RegisterAttached("RippleRadius", typeof(double), typeof(RippleHelper), new PropertyMetadata(100d));


        public static Color GetRippleColor(UIElement obj)
        {
            return (Color)obj.GetValue(RippleColorProperty);
        }

        public static void SetRippleColor(UIElement obj, Color value)
        {
            obj.SetValue(RippleColorProperty, value);
        }

        public static readonly DependencyProperty RippleColorProperty =
            DependencyProperty.RegisterAttached("RippleColor", typeof(Color), typeof(RippleHelper), new PropertyMetadata(Colors.White));

        public static RippleHelperState GetRippleHelperState(UIElement obj)
        {
            return (RippleHelperState)obj.GetValue(RippleHelperStateProperty);
        }

        public static void SetRippleHelperState(UIElement obj, RippleHelperState value)
        {
            obj.SetValue(RippleHelperStateProperty, value);
        }

        public static readonly DependencyProperty RippleHelperStateProperty =
            DependencyProperty.RegisterAttached("RippleHelperState", typeof(RippleHelperState), typeof(RippleHelper), new PropertyMetadata(RippleHelperState.None, (s, e) =>
            {
                if (e.NewValue != null && e.OldValue != e.NewValue)
                {
                    var value = (RippleHelperState)e.NewValue;
                    var oldvalue = (RippleHelperState)e.OldValue;
                    if (s is UIElement ele)
                    {
                        switch (value)
                        {
                            case RippleHelperState.Pressed:
                                {
                                    ele.RemoveHandler(UIElement.PointerReleasedEvent, pointerEventHandler);
                                    ele.AddHandler(UIElement.PointerPressedEvent, pointerEventHandler, true);
                                }
                                break;
                            case RippleHelperState.Released:
                                {
                                    ele.RemoveHandler(UIElement.PointerPressedEvent, pointerEventHandler);
                                    ele.AddHandler(UIElement.PointerReleasedEvent, pointerEventHandler, true);
                                }
                                break;
                            case RippleHelperState.None:
                                {
                                    ele.RemoveHandler(UIElement.PointerPressedEvent, pointerEventHandler);
                                    ele.RemoveHandler(UIElement.PointerReleasedEvent, pointerEventHandler);
                                    ElementCompositionPreview.SetElementChildVisual(ele, null);
                                }
                                break;
                        }
                    }
                }
            }
            ));

        #endregion

        #region EventHandle

        public static event EventHandler RippleComplated;

        private static void OnRippleComplated(UIElement ele)
        {
            RippleComplated?.Invoke(ele, EventArgs.Empty);
        }

        #endregion

        #region Method

        public static void StartRippleAnimation(UIElement ele, Vector2 position)
        {
            StartRippleAnimation(ele, position, GetRippleColor(ele), GetIsFillEnable(ele), GetRippleDuration(ele), GetRippleRadius(ele));
        }

        public static void StartRippleAnimation(UIElement ele, Vector2 position, Color color, bool isFillEnable, TimeSpan duration, double radius = 0)
        {

            var hostVisual = ElementCompositionPreview.GetElementVisual(ele);
            var compositor = Window.Current.Compositor;
            var cVisual = ElementCompositionPreview.GetElementChildVisual(ele) as ContainerVisual;
            if (cVisual == null)
            {
                cVisual = compositor.CreateContainerVisual();
                SizeBind.ClearParameter("hostVisual");
                SizeBind.SetReferenceParameter("hostVisual", hostVisual);
                cVisual.StartAnimation("Size", SizeBind);
                cVisual.Clip = compositor.CreateInsetClip();
                ElementCompositionPreview.SetElementChildVisual(ele, cVisual);
            }

            var sVisual = CreateSpriteVisual(ele, color);
            cVisual.Children.InsertAtTop(sVisual);
            sVisual.Offset = new Vector3(position.X, position.Y, 0f);

            if (isFillEnable)
            {
                var nWidth = Math.Max(Math.Max(position.X, ele.RenderSize.Width - position.X), Math.Max(position.Y, ele.RenderSize.Height - position.Y));
                var r = Math.Sqrt(nWidth * nWidth * 2);
                var finalScale = (float)r / 45f;
                PropSet.InsertScalar("ScaleValue", finalScale);
                ScaleAnimation.Duration = TimeSpan.FromMilliseconds(400);
                OpacityAnimation.Duration = TimeSpan.FromMilliseconds(430);
            }
            else
            {
                if (radius == 100d)
                {
                    PropSet.InsertScalar("ScaleValue", 2f);
                }
                else
                {
                    PropSet.InsertScalar("ScaleValue", (float)GetRippleRadius(ele) / 45f);
                }
            }

            ScaleAnimation.Duration = duration;
            OpacityAnimation.Duration = duration;

            var batch = compositor.GetCommitBatch(CompositionBatchTypes.Animation);
            batch.Completed += (s1, e1) =>
            {
                OnRippleComplated(ele);
                cVisual.Children.Remove(sVisual);
            };
            sVisual.StartAnimationGroup(RippleAnimationGroup);
        }

        private static SpriteVisual CreateSpriteVisual(UIElement ele, Color color)
        {
            var compositor = Window.Current.Compositor;
            var sVisual = compositor.CreateSpriteVisual();
            sVisual.AnchorPoint = new Vector2(0.5f, 0.5f);
            sVisual.Size = new Vector2(100f, 100f);
            var MaskBrush = compositor.CreateMaskBrush();
            MaskBrush.Mask = Mask;
            MaskBrush.Source = compositor.CreateColorBrush(color);
            sVisual.Brush = MaskBrush;
            sVisual.Size = new Vector2(100f, 100f);
            return sVisual;
        }

        private static void Ele_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UIElement ele)
            {
                var position = e.GetCurrentPoint(ele).Position.ToVector2();
                StartRippleAnimation(ele, position);
            }
        }

        private static void Ele_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is UIElement ele)
            {
                var position = e.GetCurrentPoint(ele).Position.ToVector2();
                StartRippleAnimation(ele, position);
            }
        }
        #endregion
    }

    public enum RippleHelperState
    {
        Pressed, Released, None
    }
}
