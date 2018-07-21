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
    public class OpacityHideTransition : HideTransitionBase
    {
        public OpacityHideTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        ScalarKeyFrameAnimation opacity;

        protected override void OnConnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);

            var step = host.Compositor.CreateStepEasingFunction();

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertExpressionKeyFrame(0f, "this.StartingValue");
            opacity.InsertKeyFrame(0.99f, 0f);
            opacity.InsertKeyFrame(1f, 0f, step);
            opacity.Duration = Duration;
            opacity.Target = "Opacity";

            Animation = opacity;
        }

        private void DurationPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (opacity != null)
            {
                opacity.Duration = Duration;
            }
        }
    }
}
