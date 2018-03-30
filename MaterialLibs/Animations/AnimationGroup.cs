using MaterialLibs.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Animations
{
    public class AnimationGroup : DependencyObject, IAnimationBase, IList<Animation>
    {
        public AnimationGroup()
        {
            _Factory = new AnimationGroupFactory();
            _Factory.Create().Target(Target);
        }

        private AnimationGroupFactory _Factory;
        private WeakReferenceList<UIElement> _ShowAnimationUIElements = new WeakReferenceList<UIElement>();
        private WeakReferenceList<UIElement> _HideAnimationUIElements = new WeakReferenceList<UIElement>();

        private List<Animation> animationlist = new List<Animation>();

        public Compositor Compositor => Window.Current.Compositor;
        public ICompositionAnimationBase ContentAnimation => _Factory.ContentAnimation;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateGroup();
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

        private void UpdateGroup()
        {
            _Factory.Create().Target(Target);
            var animations = (CompositionAnimationGroup)ContentAnimation;
            animations.RemoveAll();
            foreach (var animation in animationlist)
            {
                animations.Add((CompositionAnimation)animation.ContentAnimation);
            }
            foreach (var item in _ShowAnimationUIElements)
            {
                if (item != null)
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

        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Animation), new PropertyMetadata(null, TargetPropertyChanged));

        private static void TargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string target)
            {
                if (d is AnimationGroup sender)
                {
                    sender.UpdateGroup();
                }
            }
        }

        public int Count => animationlist.Count;

        public bool IsReadOnly => false;
        public Animation this[int index]
        {
            get => animationlist[index];
            set
            {
                animationlist[index].PropertyChanged -= Item_PropertyChanged;
                animationlist[index] = value;
                animationlist[index].PropertyChanged += Item_PropertyChanged;
                UpdateGroup();
            }
        }
        public int IndexOf(Animation item)
        {
            return animationlist.IndexOf(item);
        }

        public void Insert(int index, Animation item)
        {
            animationlist[index].PropertyChanged += Item_PropertyChanged;
            animationlist.Insert(index, item);
            UpdateGroup();
        }

        public void RemoveAt(int index)
        {
            animationlist[index].PropertyChanged -= Item_PropertyChanged;
            animationlist.RemoveAt(index);
            UpdateGroup();
        }

        public void Add(Animation item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            animationlist.Add(item);
            UpdateGroup();
        }

        public void Clear()
        {
            foreach (var item in animationlist)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            animationlist.Clear();
            UpdateGroup();
        }

        public bool Contains(Animation item)
        {
            return animationlist.Contains(item);
        }

        public void CopyTo(Animation[] array, int arrayIndex)
        {
            animationlist.CopyTo(array, arrayIndex);
        }

        public bool Remove(Animation item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            var result = animationlist.Remove(item);
            if (result)
            {
                UpdateGroup();
            }
            return result;
        }

        public IEnumerator<Animation> GetEnumerator()
        {
            return animationlist.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return animationlist.GetEnumerator();
        }
    }
}
