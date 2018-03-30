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
    public class AnimationCollection : DependencyObject, IList<IAnimationBase>
    {
        public static Compositor Compositor => Window.Current.Compositor;
        private readonly List<IAnimationBase> implicitAnimationlist = new List<IAnimationBase>();
        private WeakReferenceList<Visual> _hostVisuals;
        public WeakReferenceList<Visual> HostVisuals
        {
            get
            {
                if (_hostVisuals == null) _hostVisuals = new WeakReferenceList<Visual>();
                return _hostVisuals;
            }
            internal set => _hostVisuals = value;
        }

        private void UpdateAnimations()
        {
            foreach (var visual in HostVisuals)
            {
                if (visual != null)
                    visual.ImplicitAnimations = GetImplicitAnimations();
            }
        }

        internal ImplicitAnimationCollection GetImplicitAnimations()
        {
            var ImplicitAnimations = Compositor.CreateImplicitAnimationCollection();
            foreach (var an in implicitAnimationlist)
            {
                ImplicitAnimations[an.Target] = an.ContentAnimation;
            }
            return ImplicitAnimations;
        }

        internal void SetImplicitAnimations(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            HostVisuals.Add(visual);
            visual.ImplicitAnimations = GetImplicitAnimations();
        }

        internal void RemoveImplicitAnimations(UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            HostVisuals.Remove(visual);
            visual.ImplicitAnimations = null;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            UpdateAnimations();
        }

        public bool ContainsTranslationAnimation => implicitAnimationlist.Count(x => x.Target.StartsWith("Translation")) > 0;

        public IAnimationBase this[int index]
        {
            get => implicitAnimationlist[index];
            set
            {
                implicitAnimationlist[index].PropertyChanged -= Item_PropertyChanged;
                implicitAnimationlist[index] = value;
                implicitAnimationlist[index].PropertyChanged += Item_PropertyChanged;
                UpdateAnimations();
            }
        }

        public int Count => implicitAnimationlist.Count;

        public bool IsReadOnly => false;

        public void Add(IAnimationBase item)
        {
            if (!string.IsNullOrWhiteSpace(item.Target))
            {
                item.PropertyChanged += Item_PropertyChanged;
                implicitAnimationlist.Add(item);
                UpdateAnimations();
            }
        }

        public void Clear()
        {
            foreach (var item in implicitAnimationlist)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            implicitAnimationlist.Clear();
            UpdateAnimations();
        }

        public bool Contains(IAnimationBase item)
        {
            return implicitAnimationlist.Contains(item);
        }

        public void CopyTo(IAnimationBase[] array, int arrayIndex)
        {
            implicitAnimationlist.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IAnimationBase> GetEnumerator()
        {
            return implicitAnimationlist.GetEnumerator();
        }

        public int IndexOf(IAnimationBase item)
        {
            return implicitAnimationlist.IndexOf(item);
        }

        public void Insert(int index, IAnimationBase item)
        {
            if (!string.IsNullOrWhiteSpace(item.Target))
            {
                item.PropertyChanged += Item_PropertyChanged;
                implicitAnimationlist.Insert(index, item);
                UpdateAnimations();
            }
        }

        public bool Remove(IAnimationBase item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            var result = implicitAnimationlist.Remove(item);
            if (result)
            {
                UpdateAnimations();
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            implicitAnimationlist[index].PropertyChanged -= Item_PropertyChanged;
            implicitAnimationlist.RemoveAt(index);
            UpdateAnimations();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return implicitAnimationlist.GetEnumerator();
        }
    }
}
