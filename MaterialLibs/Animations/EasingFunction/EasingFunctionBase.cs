using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations.EasingFunction
{
    public class EasingFunctionBase : DependencyObject, INotifyPropertyChanged
    {
        internal EasingFunctionBase() { }
        internal CompositionEasingFunction _EasingFunction;

        public Compositor Compositor => Window.Current.Compositor;
        public CompositionEasingFunction EasingFunction => _EasingFunction;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
        }
    }
}
