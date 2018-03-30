using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations.EasingFunction
{
    public class StepEasingFunction : EasingFunctionBase
    {
        public StepEasingFunction()
        {
            _EasingFunction = Compositor.CreateStepEasingFunction(StepCount);
        }

        public int StepCount
        {
            get { return (int)GetValue(StepCountProperty); }
            set { SetValue(StepCountProperty, value); }
        }

        public static readonly DependencyProperty StepCountProperty =
            DependencyProperty.Register("StepCount", typeof(int), typeof(StepEasingFunction), new PropertyMetadata(2, StepCountPropertyChanged));

        private static void StepCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is int Count)
            {
                if (d is StepEasingFunction sender)
                {
                    sender._EasingFunction = sender.Compositor.CreateStepEasingFunction(Count);
                    sender.OnPropertyChanged();
                }
            }
        }
    }
}
