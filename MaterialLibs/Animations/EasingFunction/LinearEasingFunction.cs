using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Animations.EasingFunction
{
    public class LinearEasingFunction : EasingFunctionBase
    {
        public LinearEasingFunction()
        {
            _EasingFunction = Compositor.CreateLinearEasingFunction();
        }
    }
}
