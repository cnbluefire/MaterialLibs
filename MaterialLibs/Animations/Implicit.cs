using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Animations
{
    public class Implicit : DependencyObject
    {
        public static Compositor Compositor => Window.Current.Compositor;
        public static AnimationCollection GetAnimations(UIElement obj)
        {
            var collection = (AnimationCollection)obj.GetValue(AnimationsProperty);
            if (collection == null)
            {
                collection = new AnimationCollection();
                obj.SetValue(AnimationsProperty, collection);
            }
            return collection;
        }

        public static void SetAnimations(UIElement obj, AnimationCollection value)
        {
            obj.SetValue(AnimationsProperty, value);
        }

        public static readonly DependencyProperty AnimationsProperty =
            DependencyProperty.RegisterAttached("Animations", typeof(AnimationCollection), typeof(Implicit), new PropertyMetadata(null, AnimationsPropertyChanged));

        private static void AnimationsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement ele)
                {
                    var NewCollection = e.NewValue as AnimationCollection;
                    var OldCollection = e.OldValue as AnimationCollection;
                    var visual = ElementCompositionPreview.GetElementVisual(ele);

                    if (e.OldValue is AnimationCollection oldCollection)
                    {
                        oldCollection.RemoveImplicitAnimations(ele);
                    }
                    if (e.NewValue is AnimationCollection newCollection)
                    {
                        newCollection.SetImplicitAnimations(ele);
                    }

                }
            }
        }

        public static IAnimationBase GetShowAnimation(UIElement obj)
        {
            return (IAnimationBase)obj.GetValue(ShowAnimationProperty);
        }

        public static void SetShowAnimation(UIElement obj, IAnimationBase value)
        {
            obj.SetValue(ShowAnimationProperty, value);
        }

        public static readonly DependencyProperty ShowAnimationProperty =
            DependencyProperty.RegisterAttached("ShowAnimation", typeof(IAnimationBase), typeof(Implicit), new PropertyMetadata(null,ShowAnimationPropertyChanged));

        private static void ShowAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement ele)
                {
                    if (e.OldValue is IAnimationBase OldAnimation)
                    {
                        OldAnimation.RemoveShowAnimation(ele);
                    }
                    if (e.NewValue is IAnimationBase NewAnimation)
                    {
                        NewAnimation.SetShowAnimation(ele);
                    }
                }
            }
        }

        public static IAnimationBase GetHideAnimation(UIElement obj)
        {
            return (IAnimationBase)obj.GetValue(HideAnimationProperty);
        }

        public static void SetHideAnimation(UIElement obj, IAnimationBase value)
        {
            obj.SetValue(HideAnimationProperty, value);
        }

        public static readonly DependencyProperty HideAnimationProperty =
            DependencyProperty.RegisterAttached("HideAnimation", typeof(IAnimationBase), typeof(Implicit), new PropertyMetadata(null, HideAnimationPropertyChanged));

        private static void HideAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is UIElement ele)
                {
                    if (e.OldValue is IAnimationBase OldAnimation)
                    {
                        OldAnimation.RemoveHideAnimation(ele);
                    }
                    if (e.NewValue is IAnimationBase NewAnimation)
                    {
                        NewAnimation.SetHideAnimation(ele);
                    }
                }
            }
        }
    }
}
