using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;

namespace MaterialLibs.Animations
{
    [ContentProperty(Name = "KeyFrames")]
    public class Animation : DependencyObject, IAnimationBase
    {
        public Animation()
        {
            _Factory = new AnimationFactory();
            _Factory.Duration(Duration).DelayTime(DelayTime).Target(Target).Create(AnimationMode, EasingFunction);
        }

        private AnimationFactory _Factory;
        private WeakReferenceList<UIElement> _ShowAnimationUIElements = new WeakReferenceList<UIElement>();
        private WeakReferenceList<UIElement> _HideAnimationUIElements = new WeakReferenceList<UIElement>();
        public Compositor Compositor => Window.Current.Compositor;
        public IAnimationFactoryBase Factory => _Factory;
        public ICompositionAnimationBase ContentAnimation => Factory.ContentAnimation;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
        }


        public void SetShowAnimation(UIElement element)
        {
            _ShowAnimationUIElements.Add(element);
            ElementCompositionPreview.SetImplicitShowAnimation(element, ContentAnimation);
        }

        public void RemoveShowAnimation(UIElement element)
        {
            _ShowAnimationUIElements.Remove(element);
            ElementCompositionPreview.SetImplicitShowAnimation(element, null);
        }

        public void SetHideAnimation(UIElement element)
        {
            _HideAnimationUIElements.Add(element);
            ElementCompositionPreview.SetImplicitHideAnimation(element, ContentAnimation);
        }

        public void RemoveHideAnimation(UIElement element)
        {
            _HideAnimationUIElements.Remove(element);
            ElementCompositionPreview.SetImplicitHideAnimation(element, null);
        }

        private void UpdateAnimation()
        {
            _Factory.Create(AnimationMode, EasingFunction, true).Target(Target);
            if (ContentAnimation != null)
            {
                var keyAnimation = (KeyFrameAnimation)ContentAnimation;
                foreach (var key in KeyFrames)
                {
                    keyAnimation.InsertExpressionKeyFrame(Convert.ToSingle(key.Progress), key.Value, EasingFunction.EasingFunction);                }
            }
            foreach(var item in _ShowAnimationUIElements)
            {
                if(item != null)
                {
                    ElementCompositionPreview.SetImplicitShowAnimation(item, ContentAnimation);
                }
            }
            foreach (var item in _HideAnimationUIElements)
            {
                if (item != null)
                {
                    ElementCompositionPreview.SetImplicitHideAnimation(item, ContentAnimation);
                }
            }
            OnPropertyChanged();
        }

        private void KeyFrames_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            UpdateAnimation();
        }
        public AnimationMode AnimationMode
        {
            get { return (AnimationMode)GetValue(AnimationModeProperty); }
            set { SetValue(AnimationModeProperty, value); }
        }
        public EasingFunction.EasingFunctionBase EasingFunction
        {
            get { return (EasingFunction.EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }
        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public TimeSpan DelayTime
        {
            get { return (TimeSpan)GetValue(DelayTimeProperty); }
            set { SetValue(DelayTimeProperty, value); }
        }
        public KeyFrameCollection KeyFrames
        {
            get
            {
                var collection = (KeyFrameCollection)GetValue(KeyFramesProperty);
                if (collection == null)
                {
                    collection = new KeyFrameCollection();
                    collection.PropertyChanged += KeyFrames_PropertyChanged;
                    SetValue(KeyFramesProperty, collection);
                }
                return collection;
            }
            set
            {
                var collection = (KeyFrameCollection)GetValue(KeyFramesProperty);
                if (collection != null)
                {
                    collection.PropertyChanged -= KeyFrames_PropertyChanged;
                }
                SetValue(KeyFramesProperty, value);
                value.PropertyChanged += KeyFrames_PropertyChanged;
            }
        }

        public static readonly DependencyProperty AnimationModeProperty =
            DependencyProperty.Register("AnimationMode", typeof(AnimationMode), typeof(Animation), new PropertyMetadata(AnimationMode.Scalar, AnimationModePropertyChanged));

        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register("EasingFunction", typeof(EasingFunction.EasingFunctionBase), typeof(Animation), new PropertyMetadata(new EasingFunction.LinearEasingFunction(), EasingFunctionPropertyChanged));

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(Animation), new PropertyMetadata(TimeSpan.FromSeconds(0.33d), DurationPropertyChanged));

        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof(TimeSpan), typeof(Animation), new PropertyMetadata(TimeSpan.Zero, DelayTimePropertyChanged));

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Animation), new PropertyMetadata(null, TargetPropertyChanged));

        public static readonly DependencyProperty KeyFramesProperty =
            DependencyProperty.Register("KeyFrames", typeof(KeyFrameCollection), typeof(Animation), new PropertyMetadata(null));

        private static void AnimationModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is AnimationMode animationMode)
            {
                if (d is Animation sender)
                {
                    sender.UpdateAnimation();
                }
            }
        }
        private static void EasingFunctionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is Animation sender)
                {
                    sender.UpdateAnimation();
                }
            }
        }
        private static void DurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is TimeSpan time)
            {
                if (d is Animation sender)
                {
                    sender._Factory.Create(sender.AnimationMode, sender.EasingFunction).Duration(time);
                }
            }
        }
        private static void DelayTimePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is TimeSpan time)
            {
                if (d is Animation sender)
                {
                    sender._Factory.Create(sender.AnimationMode, sender.EasingFunction).DelayTime(time);
                }
            }
        }
        private static void TargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string target)
            {
                if (d is Animation sender)
                {
                    sender.UpdateAnimation();
                }
            }
        }

    }
}
