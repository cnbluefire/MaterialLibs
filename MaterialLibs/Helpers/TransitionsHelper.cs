using MaterialLibs.CustomTransitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.RegisterAttached("Show", typeof(ShowTransitionBase), typeof(TransitionsHelper), new PropertyMetadata(null,ShowPropertyChanged));

        public static readonly DependencyProperty HideProperty =
            DependencyProperty.RegisterAttached("Hide", typeof(HideTransitionBase), typeof(TransitionsHelper), new PropertyMetadata(null,HidePropertyChanged));


        private static void ShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != e.OldValue)
            {
                if(d is UIElement element)
                {
                    if(e.OldValue is ShowTransitionBase oldTrans)
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
    }
}
