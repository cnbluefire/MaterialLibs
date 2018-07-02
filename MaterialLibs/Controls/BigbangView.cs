using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MaterialLibs.Controls
{
    [TemplateVisualState(GroupName = "ItemSelection", Name = "ItemsNormal")]
    [TemplateVisualState(GroupName = "ItemSelection", Name = "ItemsSelected")]
    public sealed class BigbangView : ListView
    {
        public const string RootBorderName = "RootBorder";
        public const string ContentHeaderBorderName = "ContentHeaderBorder";
        public const string ContentFooterBorderName = "ContentFooterBorder";
        public const string ItemsGridName = "ItemsGrid";
        public const string ScrollViewerName = "ScrollViewer";

        public BigbangView()
        {
            this.DefaultStyleKey = typeof(BigbangView);
            this.Loaded += BigbangView_Loaded;
            this.Unloaded += BigbangView_Unloaded;
            this.SelectionChanged += BigbangView_SelectionChanged;

            PointerPressedHandler = new PointerEventHandler(_PointerPressed);
            PointerReleasedHandler = new PointerEventHandler(_PointerReleased);
            PointerMovedHandler = new PointerEventHandler(_PointerMoved);
        }

        private Compositor compositor => Window.Current.Compositor;

        BigbangPanel _Panel = null;
        Border _RootBorder;
        Border _ContentHeaderBorder;
        Border _ContentFooterBorder;
        Grid _ItemsGrid;
        ScrollViewer _ScrollViewer;

        PointerEventHandler PointerPressedHandler;
        PointerEventHandler PointerReleasedHandler;
        PointerEventHandler PointerMovedHandler;

        private Border RootBorder
        {
            get => _RootBorder;
            set => _RootBorder = value;
        }

        private Border ContentHeaderBorder
        {
            get => _ContentHeaderBorder;
            set
            {
                if (_ContentHeaderBorder != null)
                {
                    _ContentHeaderBorder.SizeChanged -= ContentHeaderBorder_SizeChanged;
                }
                _ContentHeaderBorder = value;
                if (_ContentHeaderBorder != null)
                {
                    _ContentHeaderBorder.SizeChanged += ContentHeaderBorder_SizeChanged;
                }
            }
        }

        private Border ContentFooterBorder
        {
            get => _ContentFooterBorder;
            set
            {
                if (_ContentFooterBorder != null)
                {
                    _ContentFooterBorder.SizeChanged -= ContentFooterBorder_SizeChanged;
                }
                _ContentFooterBorder = value;
                if (_ContentFooterBorder != null)
                {
                    _ContentFooterBorder.SizeChanged += ContentFooterBorder_SizeChanged;
                }
            }
        }

        private BigbangPanel Panel
        {
            get => _Panel;
            set
            {
                if (_Panel != null)
                {
                    _Panel.CommandRectChanged -= Panel_CommandRectChanged;
                }
                _Panel = value;
                if (_Panel != null)
                {
                    _Panel.CommandRectChanged += Panel_CommandRectChanged;
                }
            }
        }

        private Grid ItemsGrid
        {
            get => _ItemsGrid;
            set => _ItemsGrid = value;
        }

        private ScrollViewer ScrollViewer
        {
            get => _ScrollViewer;
            set
            {
                if (_ScrollViewer != null)
                {
                    _ScrollViewer.RemoveHandler(UIElement.PointerPressedEvent, PointerPressedHandler);
                    _ScrollViewer.RemoveHandler(UIElement.PointerReleasedEvent, PointerReleasedHandler);
                    _ScrollViewer.RemoveHandler(UIElement.PointerCanceledEvent, PointerReleasedHandler);
                    _ScrollViewer.RemoveHandler(UIElement.PointerExitedEvent, PointerReleasedHandler);
                    _ScrollViewer.ViewChanging -= _ScrollViewer_ViewChanging;
                }
                _ScrollViewer = value;
                if (_ScrollViewer != null)
                {
                    _ScrollViewer.AddHandler(UIElement.PointerPressedEvent, PointerPressedHandler, true);
                    _ScrollViewer.AddHandler(UIElement.PointerReleasedEvent, PointerReleasedHandler, true);
                    _ScrollViewer.AddHandler(UIElement.PointerCanceledEvent, PointerReleasedHandler, true);
                    _ScrollViewer.AddHandler(UIElement.PointerExitedEvent, PointerReleasedHandler, true);
                    _ScrollViewer.ViewChanging += _ScrollViewer_ViewChanging;
                }
            }
        }

        int StartIndex = -1;
        int EndIndex = -1;
        bool? IsFirstItemHadSelected;
        bool? IsSwipeEnable;
        Point? StartPoint;
        UIElement StartContainer;

        public object ContentHeader
        {
            get { return (object)GetValue(ContentHeaderProperty); }
            set { SetValue(ContentHeaderProperty, value); }
        }

        public object ContentFooter
        {
            get { return (object)GetValue(ContentFooterProperty); }
            set { SetValue(ContentFooterProperty, value); }
        }


        public static readonly DependencyProperty ContentHeaderProperty =
            DependencyProperty.Register("ContentHeader", typeof(object), typeof(BigbangView), new PropertyMetadata(0));

        public static readonly DependencyProperty ContentFooterProperty =
            DependencyProperty.Register("ContentFooter", typeof(object), typeof(BigbangView), new PropertyMetadata(0));


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootBorder = GetTemplateChild("RootBorder") as Border;
            ContentHeaderBorder = GetTemplateChild(ContentHeaderBorderName) as Border;
            ContentFooterBorder = GetTemplateChild(ContentFooterBorderName) as Border;
            ItemsGrid = GetTemplateChild(ItemsGridName) as Grid;
            ScrollViewer = GetTemplateChild(ScrollViewerName) as ScrollViewer;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var container = base.GetContainerForItemOverride();
            if (container is UIElement ele)
            {
                ele.RemoveHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(Container_PointerEntered));
                ele.AddHandler(UIElement.PointerEnteredEvent, new PointerEventHandler(Container_PointerEntered), true);
            }
            return container;
        }

        private void OnSelectionStarted(Point Position)
        {
            var tmpIndex = GetIndexFromPosition(Position, out var item);
            if (tmpIndex == -1)
            {
                return;
            }
            if (StartIndex < 0)
            {
                StartIndex = tmpIndex;
                IsFirstItemHadSelected = SelectedItems.Contains(item);
                StartContainer = ContainerFromItem(item) as UIElement;
            }
        }

        private void OnSelecting(UIElement Container)
        {
            if (IsFirstItemHadSelected.HasValue)
            {
                var sourceList = GetSourceList();
                var tmpIndex = GetIndexFromContainer(Container, out var item, sourceList);
                if (tmpIndex == -1) return;
                if (StartIndex < 0)
                {
                    StartIndex = tmpIndex;
                    return;
                }
                else
                {
                    EndIndex = tmpIndex;
                    if (EndIndex >= 0)
                    {
                        for (int i = Math.Min(StartIndex, EndIndex); i <= Math.Max(StartIndex, EndIndex); i++)
                        {
                            if (IsFirstItemHadSelected.Value)
                            {
                                if (SelectedItems.Contains(sourceList[i])) SelectedItems.Remove(sourceList[i]);
                            }
                            else
                            {
                                if (!SelectedItems.Contains(sourceList[i])) SelectedItems.Add(sourceList[i]);
                            }

                        }
                    }

                }
            }

        }

        private void OnSelectionComplated()
        {
            if (SelectedItems.Count == 0)
            {
                VisualStateManager.GoToState(this, "ItemsNormal", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "ItemsSelected", true);
            }

            if (_Panel != null)
            {
                var sourceList = GetSourceList();
                var tmp_selected_list = SelectedItems.OrderBy(x => sourceList.IndexOf(x));
                _Panel.StartSelectIndex = sourceList.IndexOf(tmp_selected_list.FirstOrDefault());
                _Panel.EndSelectIndex = sourceList.IndexOf(tmp_selected_list.LastOrDefault());
            }
            StartIndex = -1;
            EndIndex = -1;
            IsFirstItemHadSelected = null;
            IsSwipeEnable = null;
            StartPoint = null;
            StartContainer = null;
        }

        private Rect? GetItemRect(object Item)
        {
            var itemContainer = base.ContainerFromItem(Item) as UIElement;
            if (itemContainer != null && _Panel != null)
            {
                if (_Panel.ChildrenRects.ContainsKey(itemContainer))
                {
                    return _Panel.ChildrenRects[itemContainer];
                }
            }
            return null;
        }

        private int GetIndexFromContainer(UIElement Container, out object Item, IList<object> SourceList = null)
        {
            Item = ItemFromContainer(Container);
            if (Item != null)
            {
                if (SourceList == null) SourceList = GetSourceList();
                return SourceList.IndexOf(Item);
            }
            return -1;
        }

        private int GetIndexFromPosition(Point Position, out object Item)
        {
            var sourceList = GetSourceList();

            for (int i = 0; i < sourceList.Count; i++)
            {
                var rect = GetItemRect(sourceList[i]);
                if (!rect.HasValue) break;

                if (rect.Value.Contains(Position))
                {
                    Item = sourceList[i];
                    return i;
                }
            }
            Item = null;
            return -1;
        }

        private IList<object> GetSourceList()
        {
            if (ItemsSource == null) return Items.ToList();
            else
            {
                var tmp = new List<object>();
                foreach (var item in (IEnumerable)ItemsSource)
                {
                    tmp.Add(item);
                }
                return tmp;
            }
        }

        private void Container_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (IsFirstItemHadSelected.HasValue)
            {
                if (sender is UIElement ele && ele != StartContainer)
                {
                    if (StartContainer == null)
                    {
                        StartContainer = ele;
                        StartIndex = GetIndexFromContainer(StartContainer, out var item);
                    }
                    else
                    {
                        OnSelecting(ele);
                    }
                }
            }
        }

        private void _PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            StartPoint = e.GetCurrentPoint(ItemsGrid).Position;
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                if (ItemsGrid != null)
                {
                    ItemsGrid.ManipulationMode = ManipulationModes.TranslateX;
                }
                _ScrollViewer.AddHandler(UIElement.PointerMovedEvent, PointerMovedHandler, true);

            }
            else
            {
                OnSelectionStarted(StartPoint.Value);
                IsSwipeEnable = true;
            }
        }

        private void _PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch &&
                ScrollViewer != null &&
                ScrollViewer.ExtentHeight > ScrollViewer.ViewportHeight &&
                StartPoint.HasValue)
            {
                if (!IsSwipeEnable.HasValue)
                {
                    var point = e.GetCurrentPoint(ItemsGrid).Position;
                    if (Math.Abs(point.Y - StartPoint.Value.Y) < 15)
                    {
                        if (Math.Abs(point.X - StartPoint.Value.X) > 10)
                        {
                            IsSwipeEnable = true;
                            CancelDirectManipulations();
                            OnSelectionStarted(point);
                        }
                    }
                    else
                    {
                        if (ItemsGrid != null)
                        {
                            ItemsGrid.ManipulationMode = ManipulationModes.System;
                        }
                        TryStartDirectManipulation(e.Pointer);
                        IsSwipeEnable = false;
                    }
                }
            }
        }

        private void _PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (IsSwipeEnable.HasValue)
            {
                OnSelectionComplated();
            }
            if (ItemsGrid != null)
            {
                ItemsGrid.ManipulationMode = ManipulationModes.System;
            }
            _ScrollViewer.RemoveHandler(UIElement.PointerMovedEvent, PointerMovedHandler);
        }

        private void _ScrollViewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (IsSwipeEnable.HasValue)
            {
                OnSelectionComplated();
            }
            if (ItemsGrid != null)
            {
                ItemsGrid.ManipulationMode = ManipulationModes.System;
            }
            _ScrollViewer.RemoveHandler(UIElement.PointerMovedEvent, PointerMovedHandler);
        }


        private void ContentHeaderBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Panel != null && ContentHeaderBorder != null)
                Panel.ContentHeaderHeight = e.NewSize.Height;
        }

        private void ContentFooterBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Panel != null && ContentFooterBorder != null)
                Panel.ContentFooterHeight = e.NewSize.Height;
        }

        private void BigbangView_Loaded(object sender, RoutedEventArgs e)
        {
            Panel = ItemsPanelRoot as BigbangPanel;
        }

        private void BigbangView_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= BigbangView_Loaded;
            this.Unloaded -= BigbangView_Unloaded;
            this.SelectionChanged -= BigbangView_SelectionChanged;
            Panel = null;
            ContentHeaderBorder = null;
            ContentFooterBorder = null;
            RootBorder = null;
        }

        private void BigbangView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsFirstItemHadSelected == null)
            {
                OnSelectionComplated();
            }
        }

        private void Panel_CommandRectChanged(object sender, CommandBarChangedArgs args)
        {
            switch (args.Mode)
            {
                case CommandBarMode.Header:
                    {
                        if (ContentHeaderBorder != null)
                        {
                            Canvas.SetLeft(ContentHeaderBorder, 0d);
                            Canvas.SetTop(ContentHeaderBorder, args.Top);
                        }
                    }
                    break;
                case CommandBarMode.Footer:
                    {
                        if (ContentFooterBorder != null)
                        {
                            Canvas.SetLeft(ContentFooterBorder, 0d);
                            Canvas.SetTop(ContentFooterBorder, args.Top);
                        }
                    }
                    break;
            }
        }
    }
}
