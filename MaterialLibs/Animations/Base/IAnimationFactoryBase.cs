using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace MaterialLibs.Animations
{
    public interface IAnimationFactoryBase
    {
        Compositor Compositor { get; }

        ICompositionAnimationBase ContentAnimation { get; }
    }
}
