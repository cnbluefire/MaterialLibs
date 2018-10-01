using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace MaterialLibs.Helpers
{
    public class ScrollGroupHelper : DependencyObject
    {
        

    }

    internal class GroupHeaderCollection : IList<GroupHeader>
    {
        private List<GroupHeader> _list = new List<GroupHeader>();

        public WeakReferenceSource<ScrollViewer> scrollviewer = new WeakReferenceSource<ScrollViewer>();

        public GroupHeader this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
            }
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(GroupHeader item)
        {
            _list.Add(item);
            UpdateList();
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(GroupHeader item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(GroupHeader[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<GroupHeader> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(GroupHeader item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, GroupHeader item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(GroupHeader item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private Point GetPosition(GroupHeader groupHeader)
        {
            if (scrollviewer.Source == null || groupHeader.Target == null) return new Point(0, 0);
            return groupHeader.Target.TransformToVisual(scrollviewer.Source).TransformPoint(new Point(0, 0));
        }

        private void UpdateAnimations()
        {
            Visual prevVisual = null;
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].IsLoaded)
                {
                    var visual = ElementCompositionPreview.GetElementVisual(_list[i].Target);
                    visual.StopAnimation("Offset");

                    if (prevVisual == null)
                    {
                        var exp = visual.Compositor.CreateExpressionAnimation("");
                    }
                    else
                    {

                    }
                        prevVisual = visual;
                }
            }
        }

        private void UpdateList()
        {
            foreach (var item in _list)
            {
                if (item.IsLoaded)
                {
                    item.Position = GetPosition(item);
                }
            }
            _list.Sort(new Comparison<GroupHeader>((left, right) =>
            {
                return left.Position.Y.CompareTo(right.Position.Y);
            }));

            CollectionChanged.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CollectionChanged;
    }

    public class GroupHeader : DependencyObject
    {
        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(GroupHeader), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is GroupHeader sender)
                    {
                        var old_ele = a.OldValue as FrameworkElement;
                        var new_ele = a.NewValue as FrameworkElement;

                        if (old_ele != null)
                        {
                            old_ele.Loaded -= sender.Source_Loaded;
                            old_ele.Unloaded -= sender.Source_Unloaded;
                        }
                        if (new_ele != null)
                        {
                            new_ele.Loaded += sender.Source_Loaded;
                            new_ele.Unloaded += sender.Source_Unloaded;
                            if (new_ele.Parent != null || Windows.UI.Xaml.Media.VisualTreeHelper.GetParent(new_ele) != null)
                            {
                                sender.IsLoaded = true;
                            }
                            else
                            {
                                sender.IsLoaded = false;
                            }
                        }
                        else
                        {
                            sender.IsLoaded = false;
                        }

                        sender.TargetChanged.Invoke(sender, EventArgs.Empty);
                    }
                }
            }));

        public Point Position { get; set; }

        public bool IsLoaded { get; set; }

        private void Source_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;
            Loaded.Invoke(this, EventArgs.Empty);
        }

        private void Source_Unloaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = false;
            Unloaded.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TargetChanged;
        public event EventHandler Loaded;
        public event EventHandler Unloaded;
    }

    internal class ElementPositionItem
    {
        public WeakReferenceSource<FrameworkElement> FrameworkElement { get; set; }

        public Point Position { get; set; }
    }

    internal class WeakReferenceSource<T> where T : class
    {
        private WeakReference<T> source;
        public T Source
        {
            get
            {
                if (source != null && source.TryGetTarget(out var s))
                {
                    return s;
                }
                return null;
            }
            set
            {
                if (source == null) source = new WeakReference<T>(value);
                else source.SetTarget(value);
            }
        }
    }
}
