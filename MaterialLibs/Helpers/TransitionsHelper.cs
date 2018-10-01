using MaterialLibs.Brushes;
using MaterialLibs.CustomTransitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Helpers
{
    public class TransitionsHelper : DependencyObject
    {
        public static ShowTransitionBase GetShow(UIElement obj)
        {
            return (ShowTransitionBase)obj.GetValue(ShowProperty);
        }

        public static void SetShow(UIElement obj, ShowTransitionBase value)
        {
            obj.SetValue(ShowProperty, value);
        }

        public static HideTransitionBase GetHide(UIElement obj)
        {
            return (HideTransitionBase)obj.GetValue(HideProperty);
        }

        public static void SetHide(UIElement obj, HideTransitionBase value)
        {
            obj.SetValue(HideProperty, value);
        }

        public static Brush GetBackground(FrameworkElement obj)
        {
            return (Brush)obj.GetValue(BackgroundProperty);
        }

        public static void SetBackground(FrameworkElement obj, Brush value)
        {
            obj.SetValue(BackgroundProperty, value);
        }

        public static TimeSpan GetDuration(FrameworkElement obj)
        {
            return (TimeSpan)obj.GetValue(DurationProperty);
        }

        public static void SetDuration(FrameworkElement obj, TimeSpan value)
        {
            obj.SetValue(DurationProperty, value);
        }

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(ShowTransitionBase), typeof(TransitionsHelper), new PropertyMetadata(null, ShowPropertyChanged));

        public static readonly DependencyProperty HideProperty =
            DependencyProperty.RegisterAttached("Hide", typeof(HideTransitionBase), typeof(TransitionsHelper), new PropertyMetadata(null, HidePropertyChanged));

        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.RegisterAttached("Background", typeof(Brush), typeof(TransitionsHelper), new PropertyMetadata(null, BackgroundPropertyChanged));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(TransitionsHelper), new PropertyMetadata(TimeSpan.FromSeconds(0.6d)));

        private static void ShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement element)
                {
                    if (e.OldValue is ShowTransitionBase oldTrans)
                    {
                        oldTrans.RemoveTransition(element);
                    }
                    if (e.NewValue is ShowTransitionBase newTrans)
                    {
                        newTrans.ApplyTransition(element);
                    }
                }
            }
        }

        private static void HidePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement element)
                {
                    if (e.OldValue is HideTransitionBase oldTrans)
                    {
                        oldTrans.RemoveTransition(element);
                    }
                    if (e.NewValue is HideTransitionBase newTrans)
                    {
                        newTrans.ApplyTransition(element);
                    }
                }
            }
        }

        private static void BackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is FrameworkElement sender)
                {
                    var NewBrush = e.NewValue as Brush;
                    var OldBrush = e.OldValue as Brush;

                    DependencyProperty BackgroundProperty = null;
                    if (sender is Panel)
                    {
                        BackgroundProperty = Panel.BackgroundProperty;
                    }
                    else if (sender is Control)
                    {
                        BackgroundProperty = Control.BackgroundProperty;
                    }
                    else if (sender is Shape)
                    {
                        BackgroundProperty = Shape.FillProperty;
                    }

                    if (BackgroundProperty == null) return;
                    if (sender.GetValue(BackgroundProperty) is IFluentBrush tmp_fluent)
                    {
                        sender.SetValue(BackgroundProperty, OldBrush);
                        //tmp_fluent.ClearEventList();
                        //tmp_fluent.Dispose();
                    }

                    IFluentBrush FluentBrush = null;

                    if (OldBrush is SolidColorBrush && NewBrush is SolidColorBrush)
                    {
                        FluentBrush = new FluentSolidColorBrush()
                        {
                            Duration = GetDuration(sender),
                            BaseBrush = OldBrush,
                        };
                    }
                    else if (OldBrush is LinearGradientBrush && NewBrush is SolidColorBrush ||
                             OldBrush is SolidColorBrush && NewBrush is LinearGradientBrush ||
                             OldBrush is LinearGradientBrush && NewBrush is LinearGradientBrush)
                    {
                        FluentBrush = new FluentCompositeBrush()
                        {
                            Duration = GetDuration(sender),
                            BaseBrush = OldBrush,
                        };
                    }
                    if(FluentBrush == null)
                    {
                        sender.SetValue(BackgroundProperty, NewBrush);
                        return;
                    }
                    FluentBrush.TransitionCompleted += (s, a) =>
                    {
                        sender.SetValue(BackgroundProperty, a.NewBrush);
                        //if (s is IFluentBrush tmp_brush)
                        //{
                        //    tmp_brush.ClearEventList();
                        //    tmp_brush.Dispose();
                        //}
                    };
                    sender.SetValue(BackgroundProperty, FluentBrush);
                    FluentBrush.BaseBrush = NewBrush;

                    if(NewBrush is SolidColorBrush tmp_new_brush)
                    {
                        //tmp_new_brush.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty,Col)
                    }
                }
            }
        }
    }
}
