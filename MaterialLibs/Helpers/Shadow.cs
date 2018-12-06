using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Helpers
{
    public static class Shadow
    {
        static int Index;
        static Dictionary<int, int> HashSet = new Dictionary<int, int>();

        public static UIElement GetHost(UIElement obj)
        {
            return (UIElement)obj.GetValue(HostProperty);
        }

        public static void SetHost(UIElement obj, UIElement value)
        {
            obj.SetValue(HostProperty, value);
        }

        public static double GetOffsetX(UIElement obj)
        {
            return (double)obj.GetValue(OffsetXProperty);
        }

        public static void SetOffsetX(UIElement obj, double value)
        {
            obj.SetValue(OffsetXProperty, value);
        }

        public static double GetOffsetY(UIElement obj)
        {
            return (double)obj.GetValue(OffsetYProperty);
        }

        public static void SetOffsetY(UIElement obj, double value)
        {
            obj.SetValue(OffsetYProperty, value);
        }

        public static double GetOffsetZ(UIElement obj)
        {
            return (double)obj.GetValue(OffsetZProperty);
        }

        public static void SetOffsetZ(UIElement obj, double value)
        {
            obj.SetValue(OffsetZProperty, value);
        }

        public static double GetBlurRadius(UIElement obj)
        {
            return (double)obj.GetValue(BlurRadiusProperty);
        }

        public static void SetBlurRadius(UIElement obj, double value)
        {
            obj.SetValue(BlurRadiusProperty, value);
        }

        public static double GetOpacity(UIElement obj)
        {
            return (double)obj.GetValue(OpacityProperty);
        }

        public static void SetOpacity(UIElement obj, double value)
        {
            obj.SetValue(OpacityProperty, value);
        }

        public static Color GetColor(UIElement obj)
        {
            return (Color)obj.GetValue(ColorProperty);
        }

        public static void SetColor(UIElement obj, Color value)
        {
            obj.SetValue(ColorProperty, value);
        }

        public static bool GetIsTransitionEnable(UIElement obj)
        {
            return (bool)obj.GetValue(IsTransitionEnableProperty);
        }

        public static void SetIsTransitionEnable(UIElement obj, bool value)
        {
            obj.SetValue(IsTransitionEnableProperty, value);
        }

        public static readonly DependencyProperty HostProperty =
            DependencyProperty.RegisterAttached("Host", typeof(UIElement), typeof(Shadow), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        var hash = s.GetHashCode();
                        SpriteVisual sv = null;

                        if (a.OldValue is UIElement oldHost)
                        {
                            var cv = ElementCompositionPreview.GetElementChildVisual(oldHost) as ContainerVisual;
                            if (cv != null)
                            {
                                foreach (var item in cv.Children)
                                {
                                    if (item.Properties.TryGetScalar("NameIndex", out float NameIndex) == CompositionGetValueStatus.Succeeded)
                                    {
                                        if (HashSet.TryGetValue((int)NameIndex, out int savedHash) && hash == savedHash)
                                        {
                                            sv = item as SpriteVisual;
                                            break;
                                        }
                                    }
                                }
                                if (sv != null)
                                {
                                    cv.Children.Remove(sv);
                                }
                            }
                        }
                    }
                }
            }));


        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.RegisterAttached("OffsetX", typeof(double), typeof(Shadow), new PropertyMetadata(2d, (s, a) =>
            {

            }));


        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.RegisterAttached("OffsetY", typeof(double), typeof(Shadow), new PropertyMetadata(2d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty OffsetZProperty =
            DependencyProperty.RegisterAttached("OffsetZ", typeof(double), typeof(Shadow), new PropertyMetadata(2d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.RegisterAttached("BlurRadius", typeof(double), typeof(Shadow), new PropertyMetadata(8d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.RegisterAttached("Opacity", typeof(double), typeof(Shadow), new PropertyMetadata(1d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.RegisterAttached("Color", typeof(Color), typeof(Shadow), new PropertyMetadata(Colors.Black, (s, a) =>
             {

             }));

        public static readonly DependencyProperty IsTransitionEnableProperty =
            DependencyProperty.RegisterAttached("IsTransitionEnable", typeof(bool), typeof(Shadow), new PropertyMetadata(true, (s, a) =>
             {

             }));
    }
}
