using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MaterialLibs.Factorys
{
    public class TestEventPipe<T> : EventPipeBase<T>
    {
        public TestEventPipe(object obj, string EventName) : base(obj, EventName)
        {
        }

        protected override void OnEventAttachedCore(EventAttachedArgs args)
        {

        }
    }

    public class ActionEventPipe<T> : EventPipeBase<T>
    {
        public ActionEventPipe(object obj, string EventName, Func<bool> func) : base(obj, EventName)
        {
            this.func = func;
        }

        Func<bool> func;

        protected override void OnEventAttachedCore(EventAttachedArgs args)
        {
            args.Canceled = (func?.Invoke() ?? false);
        }
    }

    public class PointerEventPipe : EventPipeBase<PointerRoutedEventArgs>
    {
        public PointerEventPipe(object obj, string EventName, bool handledEventsToo = false) : base(obj, EventName, handledEventsToo)
        {
        }

        static DateTime lastClick;

        protected override void OnEventAttachedCore(EventAttachedArgs args)
        {
            if ((DateTime.Now - lastClick).TotalSeconds > 1)
            {
                args.Canceled = true;
                lastClick = DateTime.Now;
            }
        }

        public static bool GetIsDisableMultipleClicks(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDisableMultipleClicksProperty);
        }

        public static void SetIsDisableMultipleClicks(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDisableMultipleClicksProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsDisableMultipleClicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDisableMultipleClicksProperty =
            DependencyProperty.RegisterAttached("IsDisableMultipleClicks", typeof(bool), typeof(PointerEventPipe), new PropertyMetadata(false, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (a.NewValue is true)
                        {
                            var pipe1 = new PointerEventPipe(ele, nameof(ele.PointerPressed), true);
                            //var pipe2 = new PointerEventPipe(ele, nameof(ele.PointerReleased), true);

                            pipe1.EventAttached += (s1, a1) => a1.Handled = true;
                            //pipe2.EventAttached += (s1, a1) => a1.Handled = true;
                        }
                    }
                }
            }));


    }
}
