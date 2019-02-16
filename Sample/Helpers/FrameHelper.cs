using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Simple.Helpers
{
    public static class FrameHelper
    {
        public static Type GetNavigateType(Frame obj)
        {
            return (Type)obj.GetValue(NavigateTypeProperty);
        }

        public static void SetNavigateType(Frame obj, Type value)
        {
            obj.SetValue(NavigateTypeProperty, value);
        }

        public static readonly DependencyProperty NavigateTypeProperty =
            DependencyProperty.RegisterAttached("NavigateType", typeof(Type), typeof(FrameHelper), new PropertyMetadata(null, (s, a) =>
            {
                if (s is Frame sender)
                {
                    if (a.NewValue != a.OldValue)
                    {
                        if (a.NewValue is Type type)
                        {
                            sender.Navigate(type);
                        }
                    }
                }
            }));


    }
}
