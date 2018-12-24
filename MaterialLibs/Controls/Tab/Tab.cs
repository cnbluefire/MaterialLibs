using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MaterialLibs.Controls.Tab
{
    [ContentProperty(Name = "Items")]
    public sealed class Tab : ItemsControl, IInteractionTrackerOwner
    {
        public Tab()
        {
            this.DefaultStyleKey = typeof(Tab);
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;

            PointerPressedHandler = new PointerEventHandler(_PointerPressed);
            PointerReleasedHandler = new PointerEventHandler(_PointerReleased);

            Items.VectorChanged += Items_VectorChanged;
        }

        #region Field

        private bool _IsLoaded;
        bool _IsAnimating = false;

        private Border ContentBorder;

        private CancellationTokenSource SizeChangedToken;
        private PointerEventHandler PointerPressedHandler;
        private PointerEventHandler PointerReleasedHandler;
        private ITabHeader TabHeader;

        private Compositor Compositor => Window.Current.Compositor;
        private Visual PanelVisual;
        private Visual HostVisual;
        private CompositionPropertySet ScrollPropertySet;
        private InteractionTracker tracker;
        private VisualInteractionSource tracker_source;
        private ExpressionAnimation PositionOffsetExpression;
        private ExpressionAnimation PositionScrollExpression;

        private Vector3KeyFrameAnimation PositionAnimation;

        private int NowScrollIndex = -1;
        private int LastActiveIndex = -1;

        #endregion Field

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TabHeader = GetTemplateChild("TabsHeaderView") as ITabHeader;
            ContentBorder = GetTemplateChild("ContentBorder") as Border;

            if (TabHeader != null)
            {
                TabHeader.SelectionChanged += OnHeaderSelectionChanged;
            }

            TrySetupComposition();
            SetupTracker();

            if (ContentBorder != null)
            {
                ContentBorder.AddHandler(PointerPressedEvent, PointerPressedHandler, true);
                ContentBorder.AddHandler(PointerReleasedEvent, PointerReleasedHandler, true);
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabItem;
        }

        #endregion Overrides

        #region Private Methods

        private void SetupTracker()
        {
            if (ItemsPanelRoot == null && HostVisual == null && PanelVisual == null) return;
            tracker = InteractionTracker.CreateWithOwner(Compositor, this);
            tracker_source = VisualInteractionSource.Create(HostVisual);
            tracker_source.ManipulationRedirectionMode = VisualInteractionSourceRedirectionMode.CapableTouchpadOnly;
            tracker_source.PositionXChainingMode = InteractionChainingMode.Auto;
            tracker_source.PositionYChainingMode = InteractionChainingMode.Auto;
            tracker_source.PositionXSourceMode = InteractionSourceMode.EnabledWithoutInertia;
            tracker_source.PositionYSourceMode = InteractionSourceMode.Disabled;
            tracker_source.ScaleSourceMode = InteractionSourceMode.Disabled;

            tracker.InteractionSources.Add(tracker_source);

            UpdateTrackerPosition();

            PositionOffsetExpression = Compositor.CreateExpressionAnimation("-tracker.Position.X");
            PositionOffsetExpression.SetReferenceParameter("tracker", tracker);
            PanelVisual.StartAnimation("Offset.X", PositionOffsetExpression);

            PositionScrollExpression = Compositor.CreateExpressionAnimation("Vector2(-tracker.Position.X,-tracker.Position.Y)");
            PositionScrollExpression.SetReferenceParameter("tracker", tracker);
            ScrollPropertySet.StartAnimation("Translation", PositionScrollExpression);
        }

        private int CalcIndex()
        {
            var delta = tracker.Position.X - _LastPositionX;

            if (delta > this.ActualWidth / 5)
            {
                return Math.Min(Items.Count, NowScrollIndex + 1);
            }
            else if (delta < this.ActualWidth / 5 * 4)
            {
                return Math.Max(0, NowScrollIndex);
            }
            else
            {
                return NowScrollIndex;
            }
        }

        private void TrySetupComposition()
        {
            if (TabHeader != null && ItemsPanelRoot != null)
            {
                ScrollPropertySet = Compositor.CreatePropertySet();
                ScrollPropertySet.InsertVector2("Translation", Vector2.Zero);
                TabHeader.SetTabsRootScrollPropertySet(ScrollPropertySet);

                HostVisual = ElementCompositionPreview.GetElementVisual(this);
                PanelVisual = ElementCompositionPreview.GetElementVisual(ItemsPanelRoot);

                PositionAnimation = Compositor.CreateVector3KeyFrameAnimation();
                PositionAnimation.InsertExpressionKeyFrame(0f, "this.StartingValue");
                PositionAnimation.InsertExpressionKeyFrame(1f, "Vector3(FinalValue,0f,0f)");
                PositionAnimation.Duration = TimeSpan.FromSeconds(0.35d);
                PositionAnimation.Target = "Target";
            }
        }

        private void UpdateTrackerPosition()
        {
            if (tracker != null)
            {
                tracker.MinPosition = new Vector3(Convert.ToSingle(-this.ActualWidth / 3), 0f, 0f);
                tracker.MaxPosition = new Vector3(Convert.ToSingle(ItemsPanelRoot.ActualWidth + this.ActualWidth / 3), 0f, 0f);
            }
        }

        /// <summary>
        /// 同步Tab滚动条状态
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="disableAnimation"></param>
        private void SyncSelectedIndex(int Index, bool disableAnimation = false)
        {
            if (SelectedIndex > -1)
            {
                ChangeView(Index * this.ActualWidth, disableAnimation);
            }
            else
            {
                ChangeView(0, true);
            }
        }


        private void ChangeView(double horizontalOffset, bool disableAnimation = false)
        {
            if (tracker == null) return;
            if (disableAnimation || PositionAnimation == null)
            {
                tracker.TryUpdatePosition(new Vector3(Convert.ToSingle(horizontalOffset), 0f, 0f));
            }
            else
            {
                PositionAnimation.SetScalarParameter("FinalValue", Convert.ToSingle(horizontalOffset));
                tracker.TryUpdatePositionWithAnimation(PositionAnimation);
            }
        }

        /// <summary>
        /// 更新SelectedIndex和相关属性
        /// </summary>
        /// <param name="NewIndex"></param>
        /// <param name="OldIndex"></param>
        private void UpdateSelectedIndex(int NewIndex, int OldIndex)
        {
            if (NewIndex > -1 && NewIndex != SelectedIndex)
            {
                var newContainer = ContainerFromIndex(NewIndex);
                if (newContainer is ITabItem newTabsItem)
                {
                    SelectedIndex = NewIndex;
                    SelectedItem = Items[NewIndex];
                    TabHeader.SelectedIndex = NewIndex;
                    OnSelectionChanged(NewIndex, OldIndex);
                }
                else
                {
                    OnSelectionChanged(-1, OldIndex);
                }
            }
            else
            {
                OnSelectionChanged(-1, OldIndex);
            }
        }

        private void UpdateSelectedIndex(object NewItem, object OldItem)
        {
            UpdateSelectedIndex(Items.IndexOf(NewItem), Items.IndexOf(OldItem));
        }

        /// <summary>
        /// 同步TabHeader
        /// </summary>
        /// <param name="Index"></param>
        private void SyncTabHeader(int Index)
        {
            if (_IsLoaded && Index > -1 && Index < Items.Count)
            {
                if (Index == LastActiveIndex) return;

                var newContainer = ContainerFromIndex(Index);
                TabHeader.SyncSelection(Index);
                LastActiveIndex = Index;
            }
        }

        /// <summary>
        /// 加载或卸载Container
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Load"></param>
        private void SetContainerLoadState(int Index, bool Load)
        {
            if (Index > -1 && Index < Items.Count)
            {
                var Container = ContainerFromIndex(Index);
                if(Container is TabItem tabItem)
                {
                    tabItem.Width = this.ActualWidth;
                }
                if (Container is ITabItem itabItem)
                {
                    if (Load || !Load && itabItem.UnloadItemOutsideViewport)
                    {
                        itabItem.UpdateLoadState(Load);
                    }
                }
            }
        }

        #endregion Private Methods

        #region Events Methods

        private void _PointerPressed(object sender, PointerRoutedEventArgs args)
        {
            if (args.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                if (tracker_source != null)
                {
                    try
                    {
                        var pointer = args.GetCurrentPoint(this);
                        tracker_source.TryRedirectForManipulation(pointer);
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => Debug.WriteLine(ex));
                    }
                }
            }
        }

        private void _PointerReleased(object sender, PointerRoutedEventArgs args)
        {
            //var pointer = args.GetCurrentPoint(this);
            //if (pointer.PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            //{
            //    ReleasePointerCapture(args.Pointer);
            //}
        }

        private void OnHeaderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SyncSelectedIndex(e.NewIndex);
            UpdateSelectedIndex(e.NewIndex, e.OldIndex);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var header in Items.Select(c => (ContainerFromItem(c) as TabItem)?.Header))
            {
                TabHeader.Items.Add(header);
            }
            if (Items.Count > 0)
            {
                if (SelectedIndex == -1)
                {
                    UpdateSelectedIndex(0, -1);
                }
                else
                {
                    UpdateSelectedIndex(SelectedIndex, -1);
                }
            }

            if (ItemsPanelRoot != null)
            {
                ItemsPanelRoot.SizeChanged += ItemsPanelRoot_SizeChanged;
            }

            TrySetupComposition();
            SetupTracker();

            SyncSelectedIndex(SelectedIndex, true);
            _IsLoaded = true;
            await TabHeader.OnTabsLoadedAsync();
            SyncTabHeader(SelectedIndex);
            SetContainerLoadState(SelectedIndex, true);
        }

        private void ItemsPanelRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTrackerPosition();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _IsLoaded = false;
            ScrollPropertySet.Dispose();
            ScrollPropertySet = null;
        }

        private void Items_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            if (TabHeader == null) return;
            switch (@event.CollectionChange)
            {
                case Windows.Foundation.Collections.CollectionChange.ItemInserted:
                    if (Items[(int)@event.Index] is TabItem tabItem)
                    {
                        TabHeader.Items.Add(tabItem.Header);
                    }
                    break;
                case Windows.Foundation.Collections.CollectionChange.ItemRemoved:
                    TabHeader.Items.RemoveAt((int)@event.Index);
                    break;
                case Windows.Foundation.Collections.CollectionChange.ItemChanged:
                case Windows.Foundation.Collections.CollectionChange.Reset:
                    TabHeader.Items.Clear();
                    foreach (var header in Items.Select(c => (ContainerFromItem(c) as TabItem)?.Header))
                    {
                        TabHeader.Items.Add(header);
                    }
                    break;
            }
            if (SelectedIndex == -1 && Items.Count > 0)
            {
                UpdateSelectedIndex(0, -1);
            }
        }

        private void OnViewChanging()
        {
            if (_IsLoaded && tracker != null)
            {
                var tmp = (int)(tracker.Position.X / this.ActualWidth);

                if (tmp != NowScrollIndex)//显示左侧
                {
                    if (tmp != LastActiveIndex)
                    {
                        SyncTabHeader(tmp);
                        SetContainerLoadState(NowScrollIndex - 1, true);
                        SetContainerLoadState(NowScrollIndex + 1, false);
                    }
                }
                else if (tmp + 1 != NowScrollIndex) //显示右侧
                {
                    if (tmp + 1 != LastActiveIndex)
                    {
                        SyncTabHeader(tmp + 1);
                        SetContainerLoadState(tmp + 1, true);
                        SetContainerLoadState(tmp - 1, false);
                    }
                }
                NowScrollIndex = tmp;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in Items)
            {
                if (ContainerFromItem(item) is TabItem tabsItem)
                {
                    tabsItem.Width = this.ActualWidth;
                }
            }
            TabHeader.SetTabsWidth(e.NewSize.Width);
            if (tracker != null)
            {
                tracker.MinPosition = new Vector3(Convert.ToSingle(-this.ActualWidth / 3), 0f, 0f);
                tracker.MaxPosition = new Vector3(Convert.ToSingle(ItemsPanelRoot.ActualWidth + this.ActualWidth / 3), 0f, 0f);
            }

            SizeChangedToken?.Cancel();
            SizeChangedToken = new CancellationTokenSource();
            Task.Run(() => Task.Delay(50), SizeChangedToken.Token)
                .ContinueWith((t) => SyncSelectedIndex(SelectedIndex, true), TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion Events Methods

        #region Dependency Properties

        public Color IndicatorColor
        {
            get { return (Color)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public object LeftHeader
        {
            get { return (object)GetValue(LeftHeaderProperty); }
            set { SetValue(LeftHeaderProperty, value); }
        }

        public object RightHeader
        {
            get { return (object)GetValue(RightHeaderProperty); }
            set { SetValue(RightHeaderProperty, value); }
        }

        public DataTemplate LeftHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LeftHeaderTemplateProperty); }
            set { SetValue(LeftHeaderTemplateProperty, value); }
        }

        public DataTemplate RightHeaderTemplate
        {
            get { return (DataTemplate)GetValue(RightHeaderTemplateProperty); }
            set { SetValue(RightHeaderTemplateProperty, value); }
        }

        public double IndicatorWidth
        {
            get { return (double)GetValue(IndicatorWidthProperty); }
            set { SetValue(IndicatorWidthProperty, value); }
        }

        public double IndicatorHeight
        {
            get { return (double)GetValue(IndicatorHeightProperty); }
            set { SetValue(IndicatorHeightProperty, value); }
        }


        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Tab), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is Tab sender)
                    {
                        if (sender._IsLoaded)
                        {
                            sender.UpdateSelectedIndex(a.NewValue, a.OldValue);
                        }
                    }
                }
            }));

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Tab), new PropertyMetadata(-1, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is Tab sender)
                    {
                        if (sender._IsLoaded)
                        {
                            sender.UpdateSelectedIndex((int)a.NewValue, (int)a.OldValue);
                        }
                    }
                }
            }));

        public bool SingleSelectionFollowsFocus
        {
            get { return (bool)GetValue(SingleSelectionFollowsFocusProperty); }
            set { SetValue(SingleSelectionFollowsFocusProperty, value); }
        }


        public static readonly DependencyProperty LeftHeaderProperty =
            DependencyProperty.Register("LeftHeader", typeof(object), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty RightHeaderProperty =
            DependencyProperty.Register("RightHeader", typeof(object), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftHeaderTemplateProperty =
            DependencyProperty.Register("LeftHeaderTemplate", typeof(DataTemplate), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty RightHeaderTemplateProperty =
            DependencyProperty.Register("RightHeaderTemplate", typeof(DataTemplate), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(Color), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Tab), new PropertyMetadata(null));

        public static readonly DependencyProperty IndicatorWidthProperty =
            DependencyProperty.Register("IndicatorWidth", typeof(double), typeof(Tab), new PropertyMetadata(0d));

        public static readonly DependencyProperty IndicatorHeightProperty =
            DependencyProperty.Register("IndicatorHeight", typeof(double), typeof(Tab), new PropertyMetadata(0d));

        public static readonly DependencyProperty SingleSelectionFollowsFocusProperty =
            DependencyProperty.Register("SingleSelectionFollowsFocus", typeof(bool), typeof(Tab), new PropertyMetadata(true));


        #endregion Dependency Properties

        #region Custom Events

        public event TabSelectionChangedEvent SelectionChanged;
        private void OnSelectionChanged(int NewIndex, int OldIndex)
        {
            SelectionChanged?.Invoke(this, new TabSelectionChangedEventArgs() { NewIndex = NewIndex, OldIndex = OldIndex });
        }
        #endregion Custom Events

        #region Interaction Tracker Events

        private float _LastPositionX;

        void IInteractionTrackerOwner.CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args)
        {
            _LastPositionX = sender.Position.X;
            _IsAnimating = true;
        }

        void IInteractionTrackerOwner.IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
        {
            if (_IsAnimating)
            {
                _IsAnimating = false;
                _LastPositionX = sender.Position.X;
            }
            else
            {
                UpdateSelectedIndex(CalcIndex(), SelectedIndex);
                SyncSelectedIndex(SelectedIndex, false);
            }
        }

        void IInteractionTrackerOwner.InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {
        }

        void IInteractionTrackerOwner.InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args)
        {
            if (_IsAnimating)
            {
                _IsAnimating = false;
            }
            _LastPositionX = sender.Position.X;
        }

        void IInteractionTrackerOwner.RequestIgnored(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args)
        {
        }

        void IInteractionTrackerOwner.ValuesChanged(InteractionTracker sender, InteractionTrackerValuesChangedArgs args)
        {
            OnViewChanging();
        }

        #endregion Interaction Tracker Events

    }

    public delegate void TabSelectionChangedEvent(Tab sender, TabSelectionChangedEventArgs args);

    public class TabSelectionChangedEventArgs : EventArgs
    {
        public int NewIndex { get; set; }
        public int OldIndex { get; set; }
    }
}
