using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Controls.Tab
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);

    public class SelectionChangedEventArgs : EventArgs
    {
        public int NewIndex { get; set; }
        public int OldIndex { get; set; }
    }
}
