using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.CustomTransitions
{
    public class HideTransitionBase : CustomTransitionBase
    {
        protected HideTransitionBase()
        {

        }

        protected internal override CustomTransitionMode Mode => CustomTransitionMode.Hide;
    }

}
