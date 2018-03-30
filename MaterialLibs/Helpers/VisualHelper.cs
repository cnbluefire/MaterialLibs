using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Helpers
{
    public class VisualHelper : DependencyObject
    {
        static VisualHelper()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateExpressionAnimation"))
                {
                    CenterPointBind = Compositor.CreateExpressionAnimation("Vector3(targetVisual.Size.X / 2 , targetVisual.Size.Y / 2 , targetVisual.CenterPoint.Z)");
                }
            }
        }
        static Compositor Compositor => Window.Current.Compositor;
        static ExpressionAnimation CenterPointBind;

        public static double GetOpacity(DependencyObject obj)
        {
            return (double)obj.GetValue(OpacityProperty);
        }

        public static void SetOpacity(DependencyObject obj, double value)
        {
            obj.SetValue(OpacityProperty, value);
        }

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.RegisterAttached("Opacity", typeof(double), typeof(VisualHelper), new PropertyMetadata(0, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "Opacity"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).Opacity = Convert.ToSingle(a.NewValue);
                            }
                        }
                    }
                }
            }));


        public static double GetRotationAngle(DependencyObject obj)
        {
            return (double)obj.GetValue(RotationAngleProperty);
        }

        public static void SetRotationAngle(DependencyObject obj, double value)
        {
            obj.SetValue(RotationAngleProperty, value);
        }

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.RegisterAttached("RotationAngle", typeof(double), typeof(VisualHelper), new PropertyMetadata(0, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "RotationAngle"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).RotationAngle = Convert.ToSingle(a.NewValue);
                            }
                        }
                    }
                }
            }));

        public static double GetRotationAngleInDegrees(DependencyObject obj)
        {
            return (double)obj.GetValue(RotationAngleInDegreesProperty);
        }

        public static void SetRotationAngleInDegrees(DependencyObject obj, double value)
        {
            obj.SetValue(RotationAngleInDegreesProperty, value);
        }

        public static readonly DependencyProperty RotationAngleInDegreesProperty =
            DependencyProperty.RegisterAttached("RotationAngleInDegrees", typeof(double), typeof(VisualHelper), new PropertyMetadata(0, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "RotationAngleInDegrees"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).RotationAngleInDegrees = Convert.ToSingle(a.NewValue);
                            }
                        }
                    }
                }
            }));


        public static string GetAnchorPoint(DependencyObject obj)
        {
            return (string)obj.GetValue(AnchorPointProperty);
        }

        public static void SetAnchorPoint(DependencyObject obj, string value)
        {
            obj.SetValue(AnchorPointProperty, value);
        }

        public static readonly DependencyProperty AnchorPointProperty =
            DependencyProperty.RegisterAttached("AnchorPoint", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue && a.NewValue is string value_str)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "AnchorPoint"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).AnchorPoint = value_str.ToVector2();
                            }
                        }
                    }
                }
            }));

        public static string GetSize(DependencyObject obj)
        {
            return (string)obj.GetValue(SizeProperty);
        }

        public static void SetSize(DependencyObject obj, string value)
        {
            obj.SetValue(SizeProperty, value);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue && a.NewValue is string value_str)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "Size"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).Size = value_str.ToVector2();
                            }
                        }
                    }
                }
            }));


        public static string GetCenterPoint(DependencyObject obj)
        {
            return (string)obj.GetValue(CenterPointProperty);
        }

        public static void SetCenterPoint(DependencyObject obj, string value)
        {
            obj.SetValue(CenterPointProperty, value);
        }

        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.RegisterAttached("CenterPoint", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "CenterPoint"))
                            {
                                var visual = ElementCompositionPreview.GetElementVisual(ele);
                                if (a.NewValue is string value_str)
                                {
                                    if (value_str.ToLower() == "bind")
                                    {
                                        CenterPointBind.SetReferenceParameter("targetVisual", visual);
                                        visual.StartAnimation("CenterPoint", CenterPointBind);
                                        CenterPointBind.ClearAllParameters();
                                    }
                                    else
                                    {
                                        visual.CenterPoint = (value_str + ",0f").ToVector3();
                                    }
                                }
                                else
                                {
                                    visual.CenterPoint = Vector3.Zero;
                                }
                            }
                        }
                    }
                }
            }));

        public static string GetOffset(DependencyObject obj)
        {
            return (string)obj.GetValue(OffsetProperty);
        }

        public static void SetOffset(DependencyObject obj, string value)
        {
            obj.SetValue(OffsetProperty, value);
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.RegisterAttached("Offset", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue && a.NewValue is string value_str)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "Offset"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).Offset = value_str.ToVector3();
                            }
                        }
                    }
                }
            }));


        public static string GetRotationAxis(DependencyObject obj)
        {
            return (string)obj.GetValue(RotationAxisProperty);
        }

        public static void SetRotationAxis(DependencyObject obj, string value)
        {
            obj.SetValue(RotationAxisProperty, value);
        }

        public static readonly DependencyProperty RotationAxisProperty =
            DependencyProperty.RegisterAttached("RotationAxis", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue && a.NewValue is string value_str)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "RotationAxis"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).RotationAxis = value_str.ToVector3();
                            }
                        }
                    }
                }
            }));

        public static string GetScale(DependencyObject obj)
        {
            return (string)obj.GetValue(ScaleProperty);
        }

        public static void SetScale(DependencyObject obj, string value)
        {
            obj.SetValue(ScaleProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.RegisterAttached("Scale", typeof(string), typeof(VisualHelper), new PropertyMetadata("0,0,0", (s, a) =>
            {
                if (a.NewValue != a.OldValue && a.NewValue is string value_str)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "Scale"))
                            {
                                ElementCompositionPreview.GetElementVisual(ele).Scale = value_str.ToVector3();
                            }
                        }
                    }
                }
            }));

        public static string GetClip(DependencyObject obj)
        {
            return (string)obj.GetValue(ClipProperty);
        }

        public static void SetClip(DependencyObject obj, string value)
        {
            obj.SetValue(ClipProperty, value);
        }

        public static readonly DependencyProperty ClipProperty =
            DependencyProperty.RegisterAttached("Clip", typeof(string), typeof(VisualHelper), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (!DesignMode.DesignModeEnabled)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Composition.Visual", "Clip"))
                            {
                                if (a.NewValue is string value_str)
                                {
                                    var vector4 = value_str.ToVector4();
                                    ElementCompositionPreview.GetElementVisual(ele).Clip = Compositor.CreateInsetClip(vector4.W, vector4.X, vector4.Y, vector4.Z);
                                }
                                else
                                {
                                    ElementCompositionPreview.GetElementVisual(ele).Clip = null;
                                }
                            }

                        }
                    }

                }
            }));
    }
}
