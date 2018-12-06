using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MaterialLibs.Behaviors.Shadow.TargetObservers
{
    public delegate void TargetChangedEventHandler(ITargetObserver sender, TargetChangedEventArgs args);

    public class TargetChangedEventArgs : EventArgs
    {
        public TargetChangedEventArgs(DependencyProperty property, object value)
        {
            Property = property;
            Value = value;
        }

        DependencyProperty Property { get; }
        public object Value { get; }

    }
}
