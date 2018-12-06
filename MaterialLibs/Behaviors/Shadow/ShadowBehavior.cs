using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;

namespace MaterialLibs.Behaviors.Shadow
{
    public class ShadowBehavior : Behavior
    {
        WeakReference<UIElement> host;

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }




        public string HostName
        {
            get { return (string)GetValue(HostNameProperty); }
            set { SetValue(HostNameProperty, value); }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public double OffsetZ
        {
            get { return (double)GetValue(OffsetZProperty); }
            set { SetValue(OffsetZProperty, value); }
        }

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        public bool IsTransitionEnable
        {
            get { return (bool)GetValue(IsTransitionEnableProperty); }
            set { SetValue(IsTransitionEnableProperty, value); }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        




        public static readonly DependencyProperty HostNameProperty =
            DependencyProperty.Register("HostName", typeof(string), typeof(ShadowBehavior), new PropertyMetadata(null, (s, a) =>
            {
                if(a.NewValue != a.OldValue)
                {
                    if(s is ShadowBehavior sender)
                    {
                        if(a.NewValue is string str)
                        {
                            (sender.AssociatedObject as FrameworkElement).FindName(str);
                        }
                    }
                }
            }));


        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ShadowBehavior), new PropertyMetadata(2d, (s, a) =>
            {

            }));


        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ShadowBehavior), new PropertyMetadata(2d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty OffsetZProperty =
            DependencyProperty.Register("OffsetZ", typeof(double), typeof(ShadowBehavior), new PropertyMetadata(2d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty BlurRadiusProperty =
            DependencyProperty.Register("BlurRadius", typeof(double), typeof(ShadowBehavior), new PropertyMetadata(8d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(ShadowBehavior), new PropertyMetadata(1d, (s, a) =>
            {

            }));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ShadowBehavior), new PropertyMetadata(Colors.Black, (s, a) =>
            {

            }));

        public static readonly DependencyProperty IsTransitionEnableProperty =
            DependencyProperty.Register("IsTransitionEnable", typeof(bool), typeof(ShadowBehavior), new PropertyMetadata(true, (s, a) =>
            {

            }));
    }
}
