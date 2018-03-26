using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace MaterialLibs.Helpers
{
    public class CoreSocialistValuesHelper : DependencyObject
    {
        #region Field
        private static ExpressionAnimation _SizeBind;
        private static List<string> CoreSocialistValues = new List<string>() { "富强", "民主", "文明", "和谐", "自由", "平等", "公正", "法治", "爱国", "敬业", "诚信", "友善" };
        private static List<CompositionSurfaceBrush> _CoreSocialistValuesSurfaces;
        private static Random rnd = new Random();
        private static ScalarKeyFrameAnimation _OpacityAnimation;
        private static Vector3KeyFrameAnimation _OffsetAnimation;
        #endregion

        #region Property
        private static TappedEventHandler ele_TappedEventHandler = new TappedEventHandler(ele_Tapped);
        private static Compositor compositor => Window.Current.Compositor;
        private static CanvasDevice canvasDevice => CanvasDevice.GetSharedDevice();
        private static CompositionGraphicsDevice graphicsDevice => CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);
        private static ExpressionAnimation SizeBind
        {
            get
            {
                if (_SizeBind == null)
                {
                    _SizeBind = compositor.CreateExpressionAnimation("hostVisual.Size");
                }
                return _SizeBind;

            }
        }
        private static List<CompositionSurfaceBrush> CoreSocialistValuesSurfaces
        {
            get
            {
                if (_CoreSocialistValuesSurfaces == null)
                {
                    _CoreSocialistValuesSurfaces = new List<CompositionSurfaceBrush>();
                    var _graphicsDevice = graphicsDevice;
                    foreach (var value in CoreSocialistValues)
                    {
                        var surface = _graphicsDevice.CreateDrawingSurface(new Windows.Foundation.Size(50, 30), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
                        using (var session = CanvasComposition.CreateDrawingSession(surface))
                        {
                            session.Clear(Colors.Transparent);
                            session.DrawText(value, 0f, 0f, Colors.Red, new CanvasTextFormat() { FontSize = 20 });
                            session.Flush();
                        }
                        var brush = compositor.CreateSurfaceBrush(surface);
                        _CoreSocialistValuesSurfaces.Add(brush);
                    }
                }
                return _CoreSocialistValuesSurfaces;
            }
        }
        private static ScalarKeyFrameAnimation OpacityAnimation
        {
            get
            {
                if (_OpacityAnimation == null)
                {
                    _OpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
                    _OpacityAnimation.InsertKeyFrame(0f, 0f);
                    _OpacityAnimation.InsertKeyFrame(0.4f, 1f);
                    _OpacityAnimation.InsertKeyFrame(0.6f, 1f);
                    _OpacityAnimation.InsertKeyFrame(1f, 0f);
                    _OpacityAnimation.Duration = TimeSpan.FromSeconds(1.2d);
                }
                return _OpacityAnimation;
            }
        }
        private static Vector3KeyFrameAnimation OffsetAnimation
        {
            get
            {
                if (_OffsetAnimation == null)
                {
                    _OffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
                    _OffsetAnimation.InsertExpressionKeyFrame(0f, "start");
                    _OffsetAnimation.InsertExpressionKeyFrame(0.4f, "final");
                    _OffsetAnimation.InsertExpressionKeyFrame(0.6f, "final");
                    _OffsetAnimation.InsertExpressionKeyFrame(1f, "start");
                    _OffsetAnimation.Duration = TimeSpan.FromSeconds(1.2d);
                }
                return _OffsetAnimation;
            }
        }
        #endregion

        #region AttachedProperty
        public static bool GetIsCoreSocialistValuesEnable(UIElement obj)
        {
            return (bool)obj.GetValue(IsCoreSocialistValuesEnableProperty);
        }

        public static void SetIsCoreSocialistValuesEnable(UIElement obj, bool value)
        {
            obj.SetValue(IsCoreSocialistValuesEnableProperty, value);
        }

        public static readonly DependencyProperty IsCoreSocialistValuesEnableProperty =
            DependencyProperty.RegisterAttached("IsCoreSocialistValuesEnable", typeof(bool), typeof(CoreSocialistValuesHelper), new PropertyMetadata(false, OnIsCoreSocialistValuesEnablePropertyChanged));

        private static void OnIsCoreSocialistValuesEnablePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement ele)
                {
                    var isEnable = (bool)e.NewValue;
                    if (isEnable) ele.AddHandler(UIElement.TappedEvent, ele_TappedEventHandler, true);
                    else
                    {
                        ele.RemoveHandler(UIElement.TappedEvent, ele_TappedEventHandler);
                        var cVisual = ElementCompositionPreview.GetElementChildVisual(ele) as ContainerVisual;
                        if (cVisual != null)
                        {
                            cVisual.Children.RemoveAll();
                            ElementCompositionPreview.SetElementChildVisual(ele, null);
                            cVisual.Dispose();
                        }
                    }
                }
            }
        }
        #endregion

        #region Method
        private static async void ele_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is UIElement ele)
            {
                await ele.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var point = e.GetPosition(ele).ToVector2();
                    var hostVisual = ElementCompositionPreview.GetElementVisual(ele);
                    var cVisual = ElementCompositionPreview.GetElementChildVisual(ele) as ContainerVisual;
                    if (cVisual == null)
                    {
                        cVisual = compositor.CreateContainerVisual();
                        SizeBind.SetReferenceParameter("hostVisual", hostVisual);
                        cVisual.StartAnimation("Size", SizeBind);
                        ElementCompositionPreview.SetElementChildVisual(ele, cVisual);
                    }
                    var sVisual = compositor.CreateSpriteVisual();
                    sVisual.Size = new Vector2(50f, 30f);
                    sVisual.Brush = CoreSocialistValuesSurfaces[rnd.Next(0, 12)];
                    sVisual.Opacity = 0f;
                    var start = new Vector3(point.X - 25f, point.Y, 0f);
                    var final = new Vector3(point.X - 25f, point.Y - 40f, 0f);
                    OffsetAnimation.SetVector3Parameter("start", start);
                    OffsetAnimation.SetVector3Parameter("final", final);
                    cVisual.Children.InsertAtTop(sVisual);
                    var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    sVisual.StartAnimation("Opacity", OpacityAnimation);
                    sVisual.StartAnimation("Offset", OffsetAnimation);
                    batch.Completed += (s1, a1) =>
                    {
                        var cVisual_tmp = ElementCompositionPreview.GetElementChildVisual(ele) as ContainerVisual;
                        if (cVisual_tmp != null)
                        {
                            if (cVisual_tmp.Children.Contains(sVisual))
                            {
                                cVisual_tmp.Children.Remove(sVisual);
                                sVisual.Dispose();
                            }
                        }
                    };
                    batch.End();
                });
            }
        }
        #endregion
    }
}
