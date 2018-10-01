using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Brushes
{
    public class FluentCompositeBrush : XamlCompositionBrushBase, IFluentBrush
    {
        Compositor Compositor => Window.Current.Compositor;
        CanvasDevice canvasDevice;
        CompositionGraphicsDevice graphicsDevice;
        CompositionDrawingSurface surface1;
        CompositionDrawingSurface surface2;
        CompositionSurfaceBrush surfaceBrush1;
        CompositionSurfaceBrush surfaceBrush2;
        ScalarKeyFrameAnimation Source1Animation;
        ScalarKeyFrameAnimation Source2Animation;
        CompositionColorBrush colorBrush1;
        CompositionColorBrush colorBrush2;
        bool IsConnected;

        protected override void OnConnected()
        {
            if (CompositionBrush == null)
            {
                IsConnected = true;
                canvasDevice = CanvasDevice.GetSharedDevice();
                graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(Compositor, canvasDevice);
                surface1 = graphicsDevice.CreateDrawingSurface(
                    new Windows.Foundation.Size(100, 100),
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
                surface2 = graphicsDevice.CreateDrawingSurface(
                    new Windows.Foundation.Size(100, 100),
                    Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
                surfaceBrush1 = Compositor.CreateSurfaceBrush(surface1);
                surfaceBrush2 = Compositor.CreateSurfaceBrush(surface2);
                surfaceBrush1.Stretch = CompositionStretch.Fill;
                surfaceBrush2.Stretch = CompositionStretch.Fill;

                colorBrush1 = Compositor.CreateColorBrush();
                colorBrush2 = Compositor.CreateColorBrush();

                Source1Animation = Compositor.CreateScalarKeyFrameAnimation();
                Source1Animation.InsertKeyFrame(0f, 1f);
                Source1Animation.InsertKeyFrame(1f, 0f);
                Source1Animation.Duration = Duration;

                Source2Animation = Compositor.CreateScalarKeyFrameAnimation();
                Source2Animation.InsertKeyFrame(0f, 0f);
                Source2Animation.InsertKeyFrame(1f, 1f);
                Source2Animation.Duration = Duration;

                var effect = new ArithmeticCompositeEffect()
                {
                    Name = "effect",
                    Source1 = new CompositionEffectSourceParameter("source1"),
                    Source2 = new CompositionEffectSourceParameter("source2"),
                    Source1Amount = 1f,
                    Source2Amount = 0f,
                    MultiplyAmount = 0,
                };
                CompositionBrush = Compositor.CreateEffectFactory(effect, new[] { "effect.Source1Amount", "effect.Source2Amount" }).CreateBrush();

            }
        }

        protected override void OnDisconnected()
        {
            if (CompositionBrush != null)
            {
                IsConnected = false;

                canvasDevice.Dispose();
                canvasDevice = null;
                graphicsDevice.Dispose();
                graphicsDevice = null;

                surfaceBrush1.Dispose();
                surfaceBrush1 = null;
                surfaceBrush2.Dispose();
                surfaceBrush2 = null;

                surface1.Dispose();
                surface1 = null;
                surface2.Dispose();
                surface2 = null;

                Source1Animation.Dispose();
                Source1Animation = null;
                Source2Animation.Dispose();
                Source2Animation = null;

                colorBrush1.Dispose();
                colorBrush1 = null;
                colorBrush2.Dispose();
                colorBrush2 = null;

                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        public void ClearEventList()
        {
            TransitionCompleted = null;
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(FluentCompositeBrush), new PropertyMetadata(TimeSpan.FromSeconds(0.4d), (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentCompositeBrush sender)
                    {
                        if (sender.Source1Animation != null)
                        {
                            sender.Source1Animation.Duration = (TimeSpan)a.NewValue;
                        }
                        if (sender.Source1Animation != null)
                        {
                            sender.Source1Animation.Duration = (TimeSpan)a.NewValue;
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
            DependencyProperty.Register("BaseBrush", typeof(Brush), typeof(FluentCompositeBrush), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is FluentCompositeBrush sender)
                    {
                        if (sender.CompositionBrush != null && sender.Source1Animation != null && sender.Source2Animation != null)
                        {
                            var EffecBrush = (CompositionEffectBrush)sender.CompositionBrush;

                            var OldBrush = a.OldValue as Brush;
                            var NewBrush = a.NewValue as Brush;

                            if (OldBrush is SolidColorBrush && NewBrush is LinearGradientBrush)
                            {
                                sender.UpdateSurface(sender.surface2, NewBrush as LinearGradientBrush);
                                sender.colorBrush1.Color = (OldBrush as SolidColorBrush).Color;
                                EffecBrush.SetSourceParameter("source1", sender.colorBrush1);
                                EffecBrush.SetSourceParameter("source2", sender.surfaceBrush2);
                            }
                            else if (OldBrush is LinearGradientBrush && NewBrush is SolidColorBrush)
                            {
                                sender.UpdateSurface(sender.surface1, OldBrush as LinearGradientBrush);
                                sender.colorBrush2.Color = (NewBrush as SolidColorBrush).Color;
                                EffecBrush.SetSourceParameter("source1", sender.surfaceBrush1);
                                EffecBrush.SetSourceParameter("source2", sender.colorBrush2);
                            }
                            else if (OldBrush is LinearGradientBrush && NewBrush is LinearGradientBrush)
                            {
                                sender.UpdateSurface(sender.surface1, OldBrush as LinearGradientBrush);
                                sender.UpdateSurface(sender.surface2, NewBrush as LinearGradientBrush);
                                EffecBrush.SetSourceParameter("source1", sender.surfaceBrush1);
                                EffecBrush.SetSourceParameter("source2", sender.surfaceBrush2);
                            }

                            var batch = sender.Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                            batch.Completed += (s1, a1) =>
                            {
                                var isConnected = sender.IsConnected;
                                if (isConnected)
                                {
                                    sender.OnTransitionCompleted(a.OldValue as Brush, a.NewValue as Brush);
                                }
                            };
                            sender.CompositionBrush.StartAnimation("effect.Source1Amount", sender.Source1Animation);
                            sender.CompositionBrush.StartAnimation("effect.Source2Amount", sender.Source2Animation);
                            batch.End();
                            batch.Dispose();
                        }
                    }
                }
            }));

        private void UpdateSurface(CompositionDrawingSurface surface, LinearGradientBrush source)
        {
            var stops = ToWin2DGradientStops(source.GradientStops);
            var brush = new CanvasLinearGradientBrush(canvasDevice, stops, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Premultiplied);
            brush.StartPoint = source.StartPoint.ToVector2() * 100;
            brush.EndPoint = source.EndPoint.ToVector2() * 100;
            using (var dc = CanvasComposition.CreateDrawingSession(surface))
            {
                dc.FillRectangle(0, 0, 100, 100, brush);
                dc.Flush();
            }
            brush.Dispose();
        }

        private static CanvasGradientStop[] ToWin2DGradientStops(GradientStopCollection stops)
        {
            var canvasStops = new CanvasGradientStop[stops.Count];

            int x = 0;
            foreach (var stop in stops)
            {
                canvasStops[x++] = new CanvasGradientStop() { Color = stop.Color, Position = (float)stop.Offset };
            }

            return canvasStops;
        }

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