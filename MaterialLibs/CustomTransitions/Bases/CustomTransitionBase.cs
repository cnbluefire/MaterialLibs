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
    public abstract class CustomTransitionBase : DependencyObject
    {
        protected ICompositionAnimationBase Animation { get; set; }

        internal protected abstract CustomTransitionMode Mode { get; }

        protected virtual void OnConnected(UIElement element) { }

        protected virtual void OnDisconnected(UIElement element) { }

        internal protected void ApplyTransition(UIElement element)
        {
            OnConnected(element);
            switch (Mode)
            {
                case CustomTransitionMode.Show:
                    ElementCompositionPreview.SetImplicitShowAnimation(element, Animation);
                    break;
                case CustomTransitionMode.Hide:
                    ElementCompositionPreview.SetImplicitHideAnimation(element, Animation);
                    break;
            }
        }

        internal protected void RemoveTransition(UIElement element)
        {
            switch (Mode)
            {
                case CustomTransitionMode.Show:
                    ElementCompositionPreview.SetImplicitShowAnimation(element, null);
                    break;
                case CustomTransitionMode.Hide:
                    ElementCompositionPreview.SetImplicitHideAnimation(element, null);
                    break;
            }
            OnDisconnected(element);
        }
        
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(CustomTransitionBase), new PropertyMetadata(TimeSpan.FromSeconds(0.33)));

    }

}
