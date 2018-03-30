using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
                PropSet.InsertScalar("progress", 0f);
                PropSet.InsertScalar("DeltaY", 0f);
                PropSet.InsertScalar("Threshold", Convert.ToSingle(Threshold));
                hostVisual = ElementCompositionPreview.GetElementVisual(RootBorder);
            }

        }

        private Compositor Compositor => Window.Current.Compositor;
        private CompositionPropertySet ScrollerPropSet;
        private CompositionPropertySet PropSet;
        private Visual hostVisual;
        private ExpressionAnimation OffsetExpression;
        private ExpressionAnimation ScaleExpression;
        private ExpressionAnimation OpacityExpression;
        private ExpressionAnimation ProgressExpression;
        private WeakReference<ScrollViewer> _scrollViewer;
        private double LastY;
        private HeaderState LastState;
        private float CumulativeDeltaY;
        private ScrollViewer scrollViewer
        {
            get
            {
                ScrollViewer element = null;
                _scrollViewer?.TryGetTarget(out element);
                return element;
            }
            set
            {
                if (scrollViewer != null)
                {
                    scrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
                }
                _scrollViewer = new WeakReference<ScrollViewer>(value);
                if (value != null)
                {
                    value.ViewChanged += ScrollViewer_ViewChanged;
                }
            }
        }
        public event ScrollHeaderStateChangedEventHandler ScrollHeaderStateChanged;

        private void OnScrollHeaderStateChanged(HeaderState State)
        {
            LastState = State;
            ScrollHeaderStateChanged?.Invoke(this, new ScrollHeaderStateChangedEventArgs(State));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadScrollerPropSet(0);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                StopAnimations();
            }
            if (scrollViewer != null)
            {
                scrollViewer.ViewChanged -= ScrollViewer_ViewChanged;
            }

        }
        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (sender is FrameworkElement element)
                {
                    element.Loaded -= Element_Loaded;
                    scrollViewer = element.VisualTreeFindName<ScrollViewer>(TargetScrollerName);
                    if (scrollViewer != null)
                    {
                        ScrollerPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
                        UpdateAnimations();
                    }
                }
            }

        }
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scroll = (ScrollViewer)sender;
            if (UseQuickBack)
            {
                var DeltaY = Convert.ToSingle(scroll.VerticalOffset - LastY);
                LastY = scroll.VerticalOffset;

                CumulativeDeltaY += DeltaY;
                if (CumulativeDeltaY <= 0)
                {
                    CumulativeDeltaY = 0;
                    if (LastState != HeaderState.Start)
                        OnScrollHeaderStateChanged(HeaderState.Start);
                }
                else if (CumulativeDeltaY >= Threshold)
                {
                    CumulativeDeltaY = Convert.ToSingle(Threshold);
                    if (LastState != HeaderState.Final)
                        OnScrollHeaderStateChanged(HeaderState.Final);
                }
                else
                {
                    OnScrollHeaderStateChanged(HeaderState.Changing);
                }
                PropSet.InsertScalar("DeltaY", CumulativeDeltaY);
            }
            else
            {
                LastY = scroll.VerticalOffset;
                if (scroll.VerticalOffset <= 0)
                {
                    if (LastState != HeaderState.Start)
                        OnScrollHeaderStateChanged(HeaderState.Start);
                }
                else if (scroll.VerticalOffset >= Threshold)
                {
                    if (LastState != HeaderState.Final)
                        OnScrollHeaderStateChanged(HeaderState.Final);
                }
                else
                {
                    OnScrollHeaderStateChanged(HeaderState.Changing);
                }
            }
        }



        private void LoadScrollerPropSet(int millisecondsDelay = 500)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                StopAnimations();
                ScrollerPropSet = null;
                scrollViewer = null;
                if (TargetScroller is FrameworkElement element)
                {
                    if (string.IsNullOrWhiteSpace(TargetScrollerName))
                    {
                        if (element is ScrollViewer tmp)
                        {
                            scrollViewer = tmp;
                        }
                    }
                    else
                    {
                        if (element is ScrollViewer tmp && TargetScrollerName.Equals(tmp.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            scrollViewer = tmp;
                        }
                        else
                        {
                            scrollViewer = element.VisualTreeFindName<ScrollViewer>(TargetScrollerName);
                            if (scrollViewer == null)
                            {
                                element.Loaded += Element_Loaded;
                            }
                        }
                    }
                }
                if (scrollViewer != null)
                {
                    ScrollerPropSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
                    UpdateAnimations(millisecondsDelay);
                }
            }
        }


        private void StopAnimations()
        {
            hostVisual.StopAnimation("Offset");
            hostVisual.StopAnimation("Scale");
            hostVisual.StopAnimation("Opacity");
            PropSet.StopAnimation("Progress");
        }

        private async void UpdateAnimations(int millisecondsDelay = 500)
        {
            if (Threshold != 0)
            {
                if (millisecondsDelay > 0)
                    await Task.Delay(500);
                UpdateProgressAnimation();
                UpdateOffsetAnimation();
                UpdateScaleAnimation();
                UpdateOpacityAnimation();
            }
        }
        private void UpdateProgressAnimation()
        {
            if (UseQuickBack)
            {
                //ProgressExpression = Compositor.CreateExpressionAnimation("Clamp(this.CurrentValue + prop.DeltaY / prop.Threshold, 0, 1)");
                ProgressExpression = Compositor.CreateExpressionAnimation("Clamp(prop.DeltaY / prop.Threshold, 0, 1)");
            }
            else
            {
                ProgressExpression = Compositor.CreateExpressionAnimation("Clamp(-scroll.Translation.Y / prop.Threshold, 0, 1)");
            }
            ProgressExpression.SetReferenceParameter("prop", PropSet);
            ProgressExpression.SetReferenceParameter("scroll", ScrollerPropSet);

            PropSet.StartAnimation("Progress", ProgressExpression);

        }
        private void UpdateOffsetAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (ScrollerPropSet != null)
                {
                    var xexp = "0f";
                    var yexp = "0f";
                    bool hasX = false;
                    bool hasY = false;

                    if (OffsetXFrom != 0d || OffsetXTo != 0d)
                    {
                        xexp = "(xTo - xFrom) * prop.Progress + xFrom";
                        hasX = true;
                    }
                    if (OffsetYFrom != 0d || OffsetYTo != 0d)
                    {

                        yexp = "(yTo - yFrom) * prop.Progress + yFrom";
                        hasY = true;
                    }

                    if (hasX || hasY)
                    {
                        OffsetExpression = Compositor.CreateExpressionAnimation($"Vector3({xexp},{yexp},0f)");
                        OffsetExpression.SetReferenceParameter("prop", PropSet);
                        if (hasX)
                        {
                            float xFrom = Convert.ToSingle(OffsetXFrom);
                            float xTo = Convert.ToSingle(OffsetXTo);
                            OffsetExpression.SetScalarParameter("xFrom", xFrom);
                            OffsetExpression.SetScalarParameter("xTo", xTo);
                        }
                        if (hasY)
                        {
                            float yFrom = Convert.ToSingle(OffsetYFrom);
                            float yTo = Convert.ToSingle(OffsetYTo);
                            OffsetExpression.SetScalarParameter("yTo", yTo);
                            OffsetExpression.SetScalarParameter("yFrom", yFrom);
                        }

                        hostVisual.StartAnimation("Offset", OffsetExpression);
                    }
                    else
                    {
                        OffsetExpression = null;
                    }
                }
            }

        }

        private void UpdateScaleAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (ScrollerPropSet != null)
                {
                    if (ScaleFrom != 0 || ScaleTo != 0)
                    {
                        float sValue = Convert.ToSingle(ScaleTo - ScaleFrom);
                        float sFrom = Convert.ToSingle(ScaleFrom);
                        var exp = "(sValue) * prop.Progress + sFrom";
                        ScaleExpression = Compositor.CreateExpressionAnimation($"Vector3({exp},{exp},1f)");
                        ScaleExpression.SetReferenceParameter("prop", PropSet);
                        ScaleExpression.SetScalarParameter("sValue", sValue);
                        ScaleExpression.SetScalarParameter("sFrom", sFrom);
                        hostVisual.StartAnimation("Scale", ScaleExpression);
                    }
                }
            }
        }

        private void UpdateOpacityAnimation()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                if (ScrollerPropSet != null)
                {
                    if (OpacityFrom != 0 || OpacityTo != 0)
                    {
                        float oValue = Convert.ToSingle(OpacityTo - OpacityFrom);
                        float oFrom = Convert.ToSingle(OpacityFrom);
                        var exp = "(oValue) * prop.Progress + oFrom";
                        OpacityExpression = Compositor.CreateExpressionAnimation(exp);
                        OpacityExpression.SetReferenceParameter("prop", PropSet);
                        OpacityExpression.SetScalarParameter("oValue", oValue);
                        OpacityExpression.SetScalarParameter("oFrom", oFrom);
                        hostVisual.StartAnimation("Opacity", OpacityExpression);
                    }
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
        public double OffsetXFrom
        {
            get { return (double)GetValue(OffsetXFromProperty); }
            set { SetValue(OffsetXFromProperty, value); }
        }
        public double OffsetXTo
        {
            get { return (double)GetValue(OffsetXToProperty); }
            set { SetValue(OffsetXToProperty, value); }
        }
        public double OffsetYFrom
        {
            get { return (double)GetValue(OffsetYFromProperty); }
            set { SetValue(OffsetYFromProperty, value); }
        }
        public double OffsetYTo
        {
            get { return (double)GetValue(OffsetYToProperty); }
            set { SetValue(OffsetYToProperty, value); }
        }
        public double ScaleFrom
        {
            get { return (double)GetValue(ScaleFromFromProperty); }
            set { SetValue(ScaleFromFromProperty, value); }
        }
        public double ScaleTo
        {
            get { return (double)GetValue(ScaleToProperty); }
            set { SetValue(ScaleToProperty, value); }
        }
        public double OpacityFrom
        {
            get { return (double)GetValue(OpacityFromProperty); }
            set { SetValue(OpacityFromProperty, value); }
        }
        public double OpacityTo
        {
            get { return (double)GetValue(OpacityToProperty); }
            set { SetValue(OpacityToProperty, value); }
        }
        public string ContentCenterPoint
        {
            get { return (string)GetValue(ContentCenterPointProperty); }
            set { SetValue(ContentCenterPointProperty, value); }
        }
        public bool UseQuickBack
        {
            get { return (bool)GetValue(UseQuickBackProperty); }
            set { SetValue(UseQuickBackProperty, value); }
        }
        public double Threshold
        {
            get { return (double)GetValue(ThresholdProperty); }
            set { SetValue(ThresholdProperty, value); }
        }

        public static readonly new DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ScrollHeaderPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty TargetScrollerProperty =
            DependencyProperty.Register("TargetScroller", typeof(object), typeof(ScrollHeaderPanel), new PropertyMetadata(null, TargetScrollerPropertyChanged));

        public static readonly DependencyProperty TargetScrollerNameProperty =
            DependencyProperty.Register("TargetScrollerName", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null, TargetScrollerNamePropertyChanged));

        public static readonly DependencyProperty OffsetXFromProperty =
            DependencyProperty.Register("OffsetXFrom", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OffsetXFromPropertyChanged));

        public static readonly DependencyProperty OffsetXToProperty =
            DependencyProperty.Register("OffsetXTo", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OffsetXToPropertyChanged));

        public static readonly DependencyProperty OffsetYFromProperty =
    DependencyProperty.Register("OffsetXFrom", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OffsetYFromPropertyChanged));

        public static readonly DependencyProperty OffsetYToProperty =
            DependencyProperty.Register("OffsetXTo", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OffsetYToPropertyChanged));

        public static readonly DependencyProperty ScaleFromFromProperty =
    DependencyProperty.Register("ScaleFrom", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, ScaleFromPropertyChanged));

        public static readonly DependencyProperty ScaleToProperty =
            DependencyProperty.Register("ScaleTo", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, ScaleToPropertyChanged));

        public static readonly DependencyProperty OpacityFromProperty =
    DependencyProperty.Register("OpacityFrom", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OpacityFromPropertyChanged));

        public static readonly DependencyProperty OpacityToProperty =
            DependencyProperty.Register("OpacityTo", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, OpacityToPropertyChanged));

        public static readonly DependencyProperty ContentCenterPointProperty =
            DependencyProperty.Register("ContentCenterPoint", typeof(string), typeof(ScrollHeaderPanel), new PropertyMetadata(null));

        public static readonly DependencyProperty UseQuickBackProperty =
            DependencyProperty.Register("UseQuickBack", typeof(bool), typeof(ScrollHeaderPanel), new PropertyMetadata(false, UseQuickBackPropertyChanged));

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register("Threshold", typeof(double), typeof(ScrollHeaderPanel), new PropertyMetadata(0d, ThresholdPropertyChanged));


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
        private static void OffsetXFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void OffsetXToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void OffsetYFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void OffsetYToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void ScaleFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void ScaleToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void OpacityFromPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void OpacityToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.UpdateOffsetAnimation();
                }
            }
        }

        private static void UseQuickBackPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    sender.LoadScrollerPropSet();
                }
            }
        }

        private static void ThresholdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is ScrollHeaderPanel sender)
                {
                    if (!DesignMode.DesignModeEnabled)
                    {
                        sender.PropSet.InsertScalar("Threshold", Convert.ToSingle(e.NewValue));
                    }
                }
            }
        }
    }

}
