using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Brushes
{
    public interface IFluentBrush
    {
        Brush BaseBrush { get; set; }
        event TransitionCompletedEventHandler TransitionCompleted;
    }
}
