using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MaterialLibs.Styles
{
    public class ChromeFlyoutMenuResources : ResourceDictionary
    {
        public ChromeFlyoutMenuResources()
        {
            this.Source = new Uri("ms-appx:///MaterialLibs/Styles/ChromeFlyoutMenu.xaml");
        }
    }
}
