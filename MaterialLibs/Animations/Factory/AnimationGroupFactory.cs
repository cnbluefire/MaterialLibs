using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations
{
    public class AnimationGroupFactory : IAnimationFactoryBase
    {
        public Compositor Compositor => Window.Current.Compositor;
        private CompositionAnimationGroup _CompositionAnimation;
        private string _Target;

        public ICompositionAnimationBase ContentAnimation
        {
            get => _CompositionAnimation;
        }

        public AnimationGroupFactory Create(bool rebuild = false)
        {
            if (rebuild || _CompositionAnimation == null)
            {
                _CompositionAnimation = Compositor.CreateAnimationGroup();
            }
            return this;
        }

        public AnimationGroupFactory Target(string target)
        {
            if (!string.IsNullOrWhiteSpace(target))
            {
                _Target = target;
            }
            return this;
        }
    }

}
