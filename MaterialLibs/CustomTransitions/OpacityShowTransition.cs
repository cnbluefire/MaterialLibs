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
    public class OpacityShowTransition :ShowTransitionBase
    {
        public OpacityShowTransition()
        {
            RegisterPropertyChangedCallback(DurationProperty, DurationPropertyChanged);
        }

        ScalarKeyFrameAnimation opacity;

        protected override void OnConnected(UIElement element)
        {
            var host = ElementCompositionPreview.GetElementVisual(element);

            opacity = host.Compositor.CreateScalarKeyFrameAnimation();
            opacity.InsertKeyFrame(0f, 0f);
            opacity.InsertExpressionKeyFrame(1f, "this.FinalValue");
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
