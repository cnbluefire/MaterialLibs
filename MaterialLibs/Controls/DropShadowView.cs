using MaterialLibs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls
{
    public sealed class DropShadowView : ContentControl
    {
        public DropShadowView()
        {
            this.DefaultStyleKey = typeof(DropShadowView);
            if (IsSupported)
            {
                Compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                Shadow = Compositor.CreateDropShadow();
                Shadow.Opacity = 0f;
                ShadowVisual = Compositor.CreateSpriteVisual();
                ShadowVisual.Shadow = Shadow;
                ImplicitAnimationHelper.CreateAnimation<float>(Shadow, "BlurRadius", TimeSpan.FromSeconds(0.5d));
                ImplicitAnimationHelper.CreateAnimation<Vector3>(Shadow, "Offset", TimeSpan.FromSeconds(0.5d));
                ImplicitAnimationHelper.CreateAnimation<float>(Shadow, "Opacity", TimeSpan.FromSeconds(0.5d));
            }
        }

        public const string DropShadowHostName = "DropShadowHost";
        public static bool IsSupported => (!DesignMode.DesignModeEnabled) && ApiInformation.IsTypePresent("Windows.UI.Composition.DropShadow");
        private Canvas _DropShadowHost;
        private Compositor Compositor;
        private SpriteVisual ShadowVisual;
        private DropShadow Shadow;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _DropShadowHost = GetTemplateChild(DropShadowHostName) as Canvas;
            if (_DropShadowHost == null) return;
            if (IsSupported)
            {
                ElementCompositionPreview.SetElementChildVisual(_DropShadowHost, ShadowVisual);
                var exp = Compositor.CreateExpressionAnimation("host.Size");
                exp.SetReferenceParameter("host", ElementCompositionPreview.GetElementVisual(_DropShadowHost));
                ShadowVisual.StartAnimation("Size", exp);
                CreateDropShadow();
            }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            UpdateMask();
        }

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(DropShadowView), new PropertyMetadata(8d, DropShadowChanged));



        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(DropShadowView), new PropertyMetadata(2d, DropShadowChanged));



        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(DropShadowView), new PropertyMetadata(2d, DropShadowChanged));


        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DropShadowView), new PropertyMetadata(Colors.Black, DropShadowChanged));


        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(DropShadowView), new PropertyMetadata(1d, DropShadowChanged));


        private static void DropShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is DropShadowView sender)
                {
                    sender.UpdateShadowProperties();
                }
            }
        }

        private void CreateDropShadow()
        {
            UpdateMask();
            UpdateShadowProperties();
        }

        private void UpdateMask()
        {
            if (IsSupported)
            {
                CompositionBrush mask = null;
                if (Content != null)
                {
                    if (Content is Image)
                    {
                        mask = ((Image)Content).GetAlphaMask();
                    }
                    else if (Content is Shape)
                    {
                        mask = ((Shape)Content).GetAlphaMask();
                    }
                    else if (Content is TextBlock)
                    {
                        mask = ((TextBlock)Content).GetAlphaMask();
                    }

                    Shadow.Mask = mask;
                }

                Shadow.Mask = mask;
            }
        }

        private void UpdateShadowProperties()
        {
            if (_DropShadowHost != null && IsSupported)
            {
                Shadow.BlurRadius = (float)BlurRadius;
                Shadow.Color = Color;
                Shadow.Opacity = (float)ShadowOpacity;
                Shadow.Offset = new Vector3((float)OffsetX, (float)OffsetY, 0);
            }
        }
    }
}
