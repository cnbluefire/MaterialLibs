using MaterialLibs.Brushes;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Behaviors.Transitions
{
    public class BackgroundTransitionBehavior : Behavior<FrameworkElement>
    {
        Brush NowBrush;
        DependencyProperty BackgroundProperty;
        IFluentBrush FluentBrush;

        protected override void OnAttached()
        {
            if (AssociatedObject is Panel)
            {
                BackgroundProperty = Panel.BackgroundProperty;
            }
            else if (AssociatedObject is Control)
            {
                BackgroundProperty = Control.BackgroundProperty;
            }
            else if (AssociatedObject is Shape)
            {
                BackgroundProperty = Shape.FillProperty;
            }
            AssociatedObject?.SetValue(BackgroundProperty, Brush);
        }

        protected override void OnDetaching()
        {
            AssociatedObject?.SetValue(BackgroundProperty, NowBrush);
            FluentBrush = null;
        }

        private void FluentBrush_TransitionCompleted(object sender, TransitionCompletedEventArgs args)
        {
            AssociatedObject?.SetValue(BackgroundProperty, args.NewBrush);
        }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(BackgroundTransitionBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0.6d)));



        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty BrushProperty =
            DependencyProperty.Register("Brush", typeof(Brush), typeof(BackgroundTransitionBehavior), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is BackgroundTransitionBehavior sender)
                    {
                        if (sender.BackgroundProperty == null) return;

                        sender.FluentBrush = null;

                        var oldBrush = a.OldValue as Brush;
                        var newBrush = a.NewValue as Brush;

                        if (oldBrush != null && newBrush != null)
                        {
                            if (oldBrush is LinearGradientBrush && newBrush is SolidColorBrush ||
                            oldBrush is SolidColorBrush && newBrush is LinearGradientBrush ||
                            oldBrush is LinearGradientBrush && newBrush is LinearGradientBrush)
                            {
                                sender.FluentBrush = new FluentCompositeBrush()
                                {
                                    BaseBrush = oldBrush,
                                    Duration = sender.Duration,
                                };
                            }
                            else if (oldBrush is SolidColorBrush && newBrush is SolidColorBrush)
                            {
                                sender.FluentBrush = new FluentSolidColorBrush()
                                {
                                    BaseBrush = oldBrush,
                                    Duration = sender.Duration
                                };
                            }
                            else
                            {
                                sender.AssociatedObject?.SetValue(sender.BackgroundProperty, newBrush);
                            }

                            if (sender.FluentBrush != null)
                            {
                                sender.FluentBrush.TransitionCompleted += sender.FluentBrush_TransitionCompleted;
                                sender.AssociatedObject?.SetValue(sender.BackgroundProperty, sender.FluentBrush);
                                sender.FluentBrush.BaseBrush = newBrush;
                            }
                        }
                        else
                        {
                            sender.AssociatedObject?.SetValue(sender.BackgroundProperty, newBrush);
                        }
                        sender.NowBrush = newBrush;
                    }
                }
            }));


    }
}
