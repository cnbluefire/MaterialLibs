using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Helpers
{
    public class HostShadowPresenter
    {
        private static Dictionary<UIElement, Visual> TargetShadowSet = new Dictionary<UIElement, Visual>();

        public HostShadowPresenter(UIElement Host,UIElement target)
        {

        }
    }
}
