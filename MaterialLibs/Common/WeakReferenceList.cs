using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Common
{
    public class WeakReferenceList<T> : IList<T> where T : class
    {
        private List<WeakReference<T>> _list = new List<WeakReference<T>>();
        public T this[int index]
        {
            get
            {
                if (_list[index] != null)
                {
                    _list[index].TryGetTarget(out var value);
                    return value;
                }
                return null;
            }
            set => _list[index] = new WeakReference<T>(value);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(new WeakReference<T>(item));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            var tmp = _list.FirstOrDefault(x =>
            {
                x.TryGetTarget(out var target);
                if (target == item) return true;
                else return false;
            });
            if (tmp == null) return false;
            return true;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.Select(x =>
            {
                x.TryGetTarget(out var target);
                return target;
            }).ToList().CopyTo(array,arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Select(x =>
            {
                x.TryGetTarget(out var target);
                return target;
            }).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            var tmp = _list.FirstOrDefault(x =>
            {
                x.TryGetTarget(out var target);
                if (target == item) return true;
                else return false;
            });
            if (tmp == null) return -1;
            return _list.IndexOf(tmp);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index,new WeakReference<T>(item));
        }

        public bool Remove(T item)
        {
            var tmp = _list.FirstOrDefault(x =>
            {
                x.TryGetTarget(out var target);
                if (target == item) return true;
                else return false;
            });
            if (tmp == null) return false;
            return _list.Remove(tmp);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.Select(x =>
            {
                x.TryGetTarget(out var target);
                return target;
            }).GetEnumerator();
        }
    }
}
