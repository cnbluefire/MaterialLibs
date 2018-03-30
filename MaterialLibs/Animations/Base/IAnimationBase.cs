using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations
{
    public interface IAnimationBase : INotifyPropertyChanged
    {
        Compositor Compositor { get; }

        ICompositionAnimationBase ContentAnimation { get; }

        string Target { get; set; }

        void SetShowAnimation(UIElement element);
        void RemoveShowAnimation(UIElement element);
        void SetHideAnimation(UIElement element);
        void RemoveHideAnimation(UIElement element);
    }
}
