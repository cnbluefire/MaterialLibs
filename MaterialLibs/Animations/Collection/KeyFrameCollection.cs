using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace MaterialLibs.Animations
{
    public class KeyFrameCollection : IList<IAnimationKeyFrameBase>, INotifyPropertyChanged
    {
        private List<IAnimationKeyFrameBase> keyframe = new List<IAnimationKeyFrameBase>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged();
        }

        public IAnimationKeyFrameBase this[int index]
        {
            get => keyframe[index];
            set
            {
                keyframe[index] = value;
                OnPropertyChanged();
            }
        }

        public int Count => keyframe.Count;

        public bool IsReadOnly => false;

        public void Add(IAnimationKeyFrameBase item)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                item.PropertyChanged += Item_PropertyChanged;
                keyframe.Add(item);
                OnPropertyChanged();
            }
        }

        public void Clear()
        {
            foreach(var item in keyframe)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            keyframe.Clear();
            OnPropertyChanged();
        }

        public bool Contains(IAnimationKeyFrameBase item)
        {
            return keyframe.Contains(item);
        }

        public void CopyTo(IAnimationKeyFrameBase[] array, int arrayIndex)
        {
            keyframe.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IAnimationKeyFrameBase> GetEnumerator()
        {
            return keyframe.GetEnumerator();
        }

        public int IndexOf(IAnimationKeyFrameBase item)
        {
            return keyframe.IndexOf(item);
        }

        public void Insert(int index, IAnimationKeyFrameBase item)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                item.PropertyChanged += Item_PropertyChanged;
                keyframe.Insert(index, item);
                OnPropertyChanged();
            }
        }

        public bool Remove(IAnimationKeyFrameBase item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            var result = keyframe.Remove(item);
            if (result)
            {
                OnPropertyChanged();
            }
            return result;
        }

        public void RemoveAt(int index)
        {
            keyframe[index].PropertyChanged += Item_PropertyChanged;
            keyframe.RemoveAt(index);
            OnPropertyChanged();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return keyframe.GetEnumerator();
        }
    }
}
