using MaterialLibs.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Common
{
    public class ScrollHeaderStateChangedEventArgs : EventArgs
    {
        internal ScrollHeaderStateChangedEventArgs(HeaderState HeaderState)
        {
            State = HeaderState;
        }
        public HeaderState State { get;private set; }
    }

    public delegate void ScrollHeaderStateChangedEventHandler(ScrollHeaderPanel sender, ScrollHeaderStateChangedEventArgs e);

    public enum HeaderState
    {
        Start, Changing, Final
    }
}
