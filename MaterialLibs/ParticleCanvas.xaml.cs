using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace MaterialLibs
{
    public sealed partial class ParticleCanvas : UserControl
    {
        public ParticleCanvas()
        {
            this.InitializeComponent();
        }

        Compositor compositor { get => Window.Current.Compositor; }
        List<Particle> Particles = new List<Particle>();
        bool MouseHandled;
        SpinLock ParticleLock = new SpinLock();
        Vector2 PointerPosition;
        long PointerDrawCount = 0;
        bool IsPointerIn = false;
        bool _IsPointerEnable = true;
        Color _LineColor = Colors.DarkGray;
        Color _ParticleColor = Colors.Gray;

        private void ParticleCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            AddMouseHandle();
        }

        private void ParticleCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            RemoveMouseHandle();
            canvas.RemoveFromVisualTree();
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var GotLock = false;
            try
            {
                ParticleLock.Enter(ref GotLock);
                for (int i = 0; i < Particles.Count; i++)
                {
                    for (int j = i + 1; j < Particles.Count; j++)
                    {
                        var range = Particles[i].GetRange(Particles[j].Offset);
                        if (range < 120)
                        {
                            args.DrawingSession.DrawLine(Particles[i].Offset.ToVector2(), Particles[j].Offset.ToVector2(), _LineColor, (120f - range) / 80f);
                        }
                    }
                    if (_IsPointerEnable && IsPointerIn)
                    {
                        var pointerRange = Particles[i].GetRange(PointerPosition);
                        if (pointerRange < 120)
                        {
                            args.DrawingSession.DrawLine(Particles[i].Offset.ToVector2(), PointerPosition, _LineColor, (120f - pointerRange) / 80f);
                            if (pointerRange > 80)
                            {
                                var tmp = PointerPosition - Particles[i].Offset.ToVector2();
                                var pointerHead = (tmp / Math.Abs(Math.Max(tmp.X, tmp.Y)));
                                Particles[i].Offset += new Vector3(pointerHead.X, pointerHead.Y, 0);
                            }
                        }
                        if (PointerDrawCount > 36000)
                        {
                            IsPointerIn = false;
                        }
                        PointerDrawCount++;
                    }
                    args.DrawingSession.FillCircle(Particles[i].Offset.ToVector2(), Particles[i].Offset.Z / 30, _ParticleColor);
                    Particles[i].NextStep();
                }
            }
            finally
            {
                if (GotLock) ParticleLock.Exit();
            }
        }


        private void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            IsPointerIn = true;
            PointerDrawCount = 0;
            PointerPosition = Window.Current.Content.TransformToVisual(this).TransformPoint(args.CurrentPoint.Position).ToVector2();
            //PointerPosition = args.CurrentPoint.Position.ToVector2();
        }

        private void CoreWindow_PointerExited(CoreWindow sender, PointerEventArgs args)
        {
            IsPointerIn = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var GotLock = false;
            try
            {
                ParticleLock.Enter(ref GotLock);
                UpdateParticles(finalSize);
            }
            finally
            {
                if (GotLock)
                {
                    ParticleLock.Exit();
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        private int GetParticleCount(Size finalSize)
        {
            return Convert.ToInt32(Math.Min(finalSize.Width, finalSize.Height) / (10 - Density));
        }

        private void UpdateParticles(Size finalSize)
        {
            var Count = GetParticleCount(finalSize);
            var ChangedCount = Count - Particles.Count;
            var Container = finalSize.ToVector2();
            if (ChangedCount > 0)
            {
                for (int i = 0; i < ChangedCount; i++)
                {
                    Particles.Add(Particle.CreateParticle(Container));
                }
            }
            else if (ChangedCount < 0)
            {
                Particles.RemoveRange(0, -ChangedCount);
            }
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Container = new Vector3(Container.X, Container.Y, Particles[i].Container.Z);
                if (Particles[i].Offset.X > Container.X || Particles[i].Offset.Y > Container.Y) Particles[i].ResetOffset(Container);
            }
        }

        private void AddMouseHandle()
        {
            bool GotLock = false;
            try
            {
                ParticleLock.Enter(ref GotLock);
                if (!MouseHandled)
                {
                    Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
                    Window.Current.CoreWindow.PointerExited += CoreWindow_PointerExited;
                    MouseHandled = true;
                }
            }
            finally
            {
                if (GotLock) ParticleLock.Exit();
            }
        }

        private void RemoveMouseHandle()
        {
            bool GotLock = false;
            try
            {
                ParticleLock.Enter(ref GotLock);
                if (MouseHandled)
                {
                    Window.Current.CoreWindow.PointerMoved -= CoreWindow_PointerMoved;
                    Window.Current.CoreWindow.PointerExited -= CoreWindow_PointerExited;
                    MouseHandled = false;
                }
            }
            finally
            {
                if (GotLock) ParticleLock.Exit();
            }
        }
        /// <summary>
        /// Min 0
        /// Max 9
        /// </summary>
        public int Density
        {
            get { return (int)GetValue(DensityProperty); }
            set { SetValue(DensityProperty, value); }
        }

        public static readonly DependencyProperty DensityProperty =
            DependencyProperty.Register("Density", typeof(int), typeof(ParticleCanvas), new PropertyMetadata(5, DensityChanged));

        private static void DensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ParticleCanvas sender)
                {
                    bool GotLock = false;
                    try
                    {
                        sender.ParticleLock.Enter(ref GotLock);
                        sender.UpdateParticles(sender.RenderSize);
                    }
                    finally
                    {
                        if (GotLock) sender.ParticleLock.Exit();
                    }
                }
            }
        }

        public bool IsPointerEnable
        {
            get { return (bool)GetValue(IsPointerEnableProperty); }
            set { SetValue(IsPointerEnableProperty, value); }
        }

        public static readonly DependencyProperty IsPointerEnableProperty =
            DependencyProperty.Register("IsPointerEnable", typeof(bool), typeof(ParticleCanvas), new PropertyMetadata(true, IsPointerEnableChanged));

        private static void IsPointerEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ParticleCanvas sender)
                {
                    sender._IsPointerEnable = (bool)e.NewValue;
                    if (sender._IsPointerEnable)
                    {
                        sender.AddMouseHandle();
                    }
                    else
                    {
                        sender.RemoveMouseHandle();
                    }
                }
            }
        }

        public bool Paused
        {
            get { return (bool)GetValue(PausedProperty); }
            set { SetValue(PausedProperty, value); }
        }
        public static readonly DependencyProperty PausedProperty =
            DependencyProperty.Register("Paused", typeof(bool), typeof(ParticleCanvas), new PropertyMetadata(false, IsPausedChanged));

        private static void IsPausedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ParticleCanvas sender)
                {
                    var paused = (bool)e.NewValue;
                    if (paused)
                    {
                        sender.canvas.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        sender.canvas.Visibility = Visibility.Visible;
                    }
                    sender.canvas.Paused = paused;
                }
            }
        }

        public Color ParticleColor
        {
            get { return (Color)GetValue(ParticleColorProperty); }
            set { SetValue(ParticleColorProperty, value); }
        }
        public static readonly DependencyProperty ParticleColorProperty =
            DependencyProperty.Register("ParticleColor", typeof(Color), typeof(ParticleCanvas), new PropertyMetadata(Colors.Gray, ParticleColorChanged));

        private static void ParticleColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ParticleCanvas sender)
                {
                    if (e.NewValue is Color color)
                        sender._ParticleColor = color;
                }
            }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Color), typeof(ParticleCanvas), new PropertyMetadata(Colors.DarkGray, LineColorChanged));

        private static void LineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ParticleCanvas sender)
                {
                    if (e.NewValue is Color color)
                        sender._LineColor = color;
                }
            }
        }
    }
}
