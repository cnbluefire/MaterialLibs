using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Brushes
{
    public delegate void TransitionCompletedEventHandler(object sender, TransitionCompletedEventArgs args);
    public class TransitionCompletedEventArgs : EventArgs
    {
        public Brush OldBrush { get; internal set; }
        public Brush NewBrush { get; internal set; }
    }
}
