using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations.EasingFunction
{
    public class CubicBezierEasingFunction : EasingFunctionBase
    {
        public CubicBezierEasingFunction()
        {
            CreateEasing(ControlPoint);
        }
        public Thickness ControlPoint
        {
            get { return (Thickness)GetValue(ControlPointProperty); }
            set { SetValue(ControlPointProperty, value); }
        }

        public static readonly DependencyProperty ControlPointProperty =
            DependencyProperty.Register("ControlPoint", typeof(Thickness), typeof(CubicBezierEasingFunction), new PropertyMetadata(new Thickness(0.17, 0.67, 0.63, 1), ControlPointPropertyChanged));

        private static void ControlPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is Thickness thickness)
            {
                if (d is CubicBezierEasingFunction sender)
                {
                    sender.CreateEasing(thickness);
                    sender.OnPropertyChanged();
                }
            }
        }

        private void CreateEasing(Thickness ControlPoint)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                _EasingFunction = Compositor.CreateCubicBezierEasingFunction(
                    new Vector2(Convert.ToSingle(ControlPoint.Left), Convert.ToSingle(ControlPoint.Top)),
                    new Vector2(Convert.ToSingle(ControlPoint.Right), Convert.ToSingle(ControlPoint.Bottom)));
            }
        }
    }
}
