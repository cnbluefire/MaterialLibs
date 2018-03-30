using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace MaterialLibs.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed partial class ScrollHeaderPanel : UserControl
    {
        public ScrollHeaderPanel()
        {
            this.InitializeComponent();
            this.DataContext = this;
            if (!DesignMode.DesignModeEnabled)
            {
                PropSet = Compositor.CreatePropertySet();
                hostVisual = ElementCompositionPreview.GetElementVisual(RootBorder);
            }

        }

        private Compositor Compositor => Window.Current.Compositor;
        private CompositionPropertySet ScrollerPropSet;
        private Visual hostVisual;
        private ExpressionAnimation OffsetExpressionAnimation;
        private ExpressionAnimation ScaleExpressionAnimation;
        private ExpressionAnimation OpacityExpressionAnimation;


        public CompositionPropertySet PropSet { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadScrollerPropSet();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                hostVisual.StopAnimation("Offset");
                hostVisual.StopAnimation("Scale");
                hostVisual.StopAnimation("Opacity");
            }

        }

        private void LoadScrollerPropSet()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                ScrollViewer scroll = null;
                if (TargetScroller is FrameworkElement element)
                {
                    if (string.IsNullOrWhiteSpace(TargetScrollerName))
                    {
                        if (element is ScrollViewer tmp)
                        {
                            scroll = tmp;
                        }
                    }
                    else
                    {
                        if (element is ScrollViewer tmp && TargetScrollerName.Equals(tmp.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            scroll = tmp;
                        }
                        else
                        {
                            scroll = element.VisualTreeFindName<ScrollViewer>(TargetScrollerName);
                            if (scroll == null)
                            {
                                element.Loaded += Element_Loaded;
                            }
                        }
                    }
                }
                if (scroll != null)
                {
                    ScrollerPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroll);
                    UpdateAnimations();
                }
            }


        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (sender is FrameworkElement element)
                {
                    element.Loaded -= Element_Loaded;
                    var scroll = element.VisualTreeFindName<ScrollViewer>(TargetScrollerName);
                    if (scroll != null)
                    {
                        ScrollerPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroll);
                        UpdateAnimations();
                    }
                }
            }

        }

        private void UpdateAnimations()
        {
            UpdateOffsetAnimation();
            UpdateScaleAnimation();
            UpdateOpacityAnimation();
        }

        private void UpdateOffsetAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                hostVisual.StopAnimation("Offset");

                if (ScrollerPropSet != null)
                {
                    var xexp = OffsetXExpression ?? "host.Offset.X";
                    var yexp = OffsetYExpression ?? "host.Offset.Y";

                    OffsetExpressionAnimation = Compositor.CreateExpressionAnimation($"Vector3({xexp},{yexp},0f)");
                    OffsetExpressionAnimation.SetReferenceParameter("scroll", ScrollerPropSet);
                    OffsetExpressionAnimation.SetReferenceParameter("host", hostVisual);
                    OffsetExpressionAnimation.SetReferenceParameter("prop", PropSet);

                    hostVisual.StartAnimation("Offset", OffsetExpressionAnimation);
                }
            }

        }

        private void UpdateScaleAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                hostVisual.StopAnimation("Scale");

                if (ScrollerPropSet != null && !string.IsNullOrWhiteSpace(ScaleExpression))
                {
                    ScaleExpressionAnimation = Compositor.CreateExpressionAnimation($"Vector3({ScaleExpression},{ScaleExpression},1f)");
                    ScaleExpressionAnimation.SetReferenceParameter("scroll", ScrollerPropSet);
                    ScaleExpressionAnimation.SetReferenceParameter("host", hostVisual);
                    ScaleExpressionAnimation.SetReferenceParameter("prop", PropSet);

                    hostVisual.StartAnimation("Scale", ScaleExpressionAnimation);
                }
            }

        }

        private void UpdateOpacityAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                hostVisual.StopAnimation("Opacity");

                if (ScrollerPropSet != null && !string.IsNullOrWhiteSpace(OpacityExpression))
                {
                    OpacityExpressionAnimation = Compositor.CreateExpressionAnimation(OpacityExpression);
                    OpacityExpressionAnimation.SetReferenceParameter("scroll", ScrollerPropSet);
                    OpacityExpressionAnimation.SetReferenceParameter("host", hostVisual);
                    OpacityExpressionAnimation.SetReferenceParameter("prop", PropSet);

                    hostVisual.StartAnimation("Opacity", OpacityExpressionAnimation);
                }
            }

        }

        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public object TargetScroller
        {
            get { return (object)GetValue(TargetScrollerProperty); }
            set { SetValue(TargetScrollerProperty, value); }
        }
        public string TargetScrollerName
        {
            get { return (string)GetValue(TargetScrollerNameProperty); }
            set { SetValue(TargetScrollerNameProperty, value); }
        }
        public string OffsetXExpression
        {
            get { return (string)GetValue(OffsetXExpressionProperty); }
            set { SetValue(OffsetXExpressionProperty, value); }
        }
        public string OffsetYExpression
        {
            get { return (string)GetValue(OffsetYExpressionProperty); }
            set { SetValue(OffsetYExpressionProperty, value); }
        }
        public string OpacityExpression
        {
            get { return (string)GetValue(OpacityExpressionProperty); }
            set { SetValue(OpacityExpressionProperty, value); }
        }
        public string ScaleExpression
        {
            get { return (string)GetValue(ScaleExpressionProperty); }
            set { SetValue(ScaleExpressionProperty, value); }
        }
        public string ContentCenterPoint
        {
            get { return (string)GetValue(ContentCenterPointProperty); }
            set { SetValue(ContentCenterPointProperty, value); }
        }

        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ScrollHeaderPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty TargetScrollerProperty =
            DependencyProperty.Register("TargetScroller", typeof(object), typeof(ScrollHeaderPanel), new PropertyMetadata(null, TargetScrollerPropertyChanged));

        public static readonly DependencyProperty TargetScrollerNameProperty =
            DependencyProperty.Register("TargetScrollerName", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, TargetScrollerNamePropertyChanged));

        public static readonly DependencyProperty OffsetXExpressionProperty =
            DependencyProperty.Register("OffsetXExpression", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, OffsetXExpressionPropertyChanged));

        public static readonly DependencyProperty OffsetYExpressionProperty =
            DependencyProperty.Register("OffsetYExpression", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, OffsetYExpressionPropertyChanged));

        public static readonly DependencyProperty OpacityExpressionProperty =
            DependencyProperty.Register("OpacityExpression", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, OpacityExpressionPropertyChanged));

        public static readonly DependencyProperty ScaleExpressionProperty =
            DependencyProperty.Register("ScaleExpression", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, ScaleExpressionPropertyChanged));

        public static readonly DependencyProperty ContentCenterPointProperty =
            DependencyProperty.Register("ContentCenterPoint", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null));

        private static void TargetScrollerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.LoadScrollerPropSet();
                }
            }
        }
        private static void TargetScrollerNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.LoadScrollerPropSet();
                }
            }
        }
        private static void OffsetXExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }
        private static void OffsetYExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }
        private static void OpacityExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOpacityAnimation();
                }
            }
        }
        private static void ScaleExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateScaleAnimation();
                }
            }
        }
    }
}
