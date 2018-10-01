using MaterialLibs.Common;
using MaterialLibs.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MaterialLibs.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class HamburgerView : Control
    {
        public HamburgerView()
        {
            this.DefaultStyleKey = typeof(HamburgerView);
            _ItemsSelectedChangedHandler = new SelectionChangedEventHandler((s, a) =>
            {
                var tmpList = (ListView)s;
                tmpList.SelectionChanged -= _ItemsSelectedChangedHandler;
                tmpList.SelectedItem = a.RemovedItems.FirstOrDefault();
            });
        }


        bool _HasManipulated = false;
        bool _templateApplied = false;
        bool _HasCanceled = false;
        bool _IsSelectedChangedByClick = false;
        Storyboard _CloseOverlayStoryboard;
        EventHandler<object> _CloseOverlayStoryboard_Complated;
        SelectionChangedEventHandler _ItemsSelectedChangedHandler;
        double _TransDelta;

        CompositeTransform _PaneRootTransform;

        Grid _rootGrid;
        Grid _paneRoot;
        Grid _contentRoot;
        Border _headerBorder;
        Button _backButton;
        Button _hamburgerButton;
        ListView _primaryListView;
        ListView _secondaryListView;
        Rectangle _lightDismissLayer;
        Rectangle _gestureRectangle;
        ContentPresenter _leftHeaderPresenter;
        ContentPresenter _rightHeaderPresenter;

        Grid _RootGrid
        {
            get => _rootGrid;
            set
            {
                _CloseOverlayStoryboard = null;
                _rootGrid = value;
                if (_rootGrid != null)
                {
                    var group = VisualStateManager.GetVisualStateGroups(_rootGrid);
                    _CloseOverlayStoryboard = group?.FirstOrDefault(x => x.Name == "DisplayModeStates")
                        ?.Transitions
                        ?.FirstOrDefault(x => x.From == "OpenOverlay" && x.To == "CloseOverlay")
                        ?.Storyboard;
                    foreach (var i in group)
                    {
                        i.CurrentStateChanged += (s, a) =>
                        {

                        };
                    }
                }
            }
        }

        Grid _PaneRoot
        {
            get => _paneRoot;
            set
            {
                _paneRoot = value;
            }
        }

        Grid _ContentRoot
        {
            get => _contentRoot;
            set
            {
                _contentRoot = value;
            }
        }

        Border _HeaderBorder
        {
            get => _headerBorder;
            set
            {
                _headerBorder = value;
            }
        }

        Button _BackButton
        {
            get => _backButton;
            set
            {
                if (_backButton != null)
                {
                    _backButton.Click -= BackButton_Click;
                }
                _backButton = value;
                if (_backButton != null)
                {
                    _backButton.Click += BackButton_Click;
                }
            }
        }

        Button _HamburgerButton
        {
            get => _hamburgerButton;
            set
            {
                if (_hamburgerButton != null)
                {
                    _hamburgerButton.Click -= HamburgerButton_Click;
                }
                _hamburgerButton = value;
                if (_hamburgerButton != null)
                {
                    _hamburgerButton.Click += HamburgerButton_Click;
                }
            }
        }

        ListView _PrimaryListView
        {
            get => _primaryListView;
            set
            {
                if (_primaryListView != null)
                {
                    _primaryListView.ItemClick -= _ItemClick;
                }
                _primaryListView = value;
                if (_primaryListView != null)
                {
                    _primaryListView.ItemClick += _ItemClick;
                }
            }
        }

        ListView _SecondaryListView
        {
            get => _secondaryListView;
            set
            {
                if (_secondaryListView != null)
                {
                    _secondaryListView.ItemClick -= _ItemClick;
                }
                _secondaryListView = value;
                if (_secondaryListView != null)
                {
                    _secondaryListView.ItemClick += _ItemClick;
                }
            }
        }
        Rectangle _LightDismissLayer
        {
            get => _lightDismissLayer;
            set
            {
                if (_lightDismissLayer != null)
                {
                    _lightDismissLayer.Tapped -= _LightDismissLayer_Tapped;
                }
                _lightDismissLayer = value;
                if (_lightDismissLayer != null)
                {
                    _lightDismissLayer.Tapped += _LightDismissLayer_Tapped;
                }
            }
        }
        Rectangle _GestureRectangle
        {
            get => _gestureRectangle;
            set
            {
                if (_gestureRectangle != null)
                {
                    _gestureRectangle.ManipulationStarted -= _GestureRectangle_ManipulationStarted;
                    _gestureRectangle.ManipulationDelta -= _GestureRectangle_ManipulationDelta;
                    _gestureRectangle.ManipulationCompleted -= _GestureRectangle_ManipulationCompleted;
                }
                _gestureRectangle = value;
                if (_gestureRectangle != null)
                {
                    _gestureRectangle.ManipulationStarted += _GestureRectangle_ManipulationStarted;
                    _gestureRectangle.ManipulationDelta += _GestureRectangle_ManipulationDelta;
                    _gestureRectangle.ManipulationCompleted += _GestureRectangle_ManipulationCompleted;
                }
            }
        }
        ContentPresenter _LeftHeaderPresenter
        {
            get => _leftHeaderPresenter;
            set
            {
                _leftHeaderPresenter = value;
            }
        }
        ContentPresenter _RightHeaderPresenter
        {
            get => _rightHeaderPresenter;
            set
            {
                _rightHeaderPresenter = value;
            }
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templateApplied = true;
            _RootGrid = GetTemplateChild("RootGrid") as Grid;
            _PaneRoot = GetTemplateChild("PaneRoot") as Grid;
            if (_PaneRoot != null)
            {
                _PaneRootTransform = _PaneRoot.RenderTransform as CompositeTransform;
            }
            _ContentRoot = GetTemplateChild("ContentRoot") as Grid;
            _HeaderBorder = GetTemplateChild("HeaderBorder") as Border;
            _BackButton = GetTemplateChild("BackButton") as Button;
            _HamburgerButton = GetTemplateChild("HamburgerButton") as Button;
            _PrimaryListView = GetTemplateChild("PrimaryListView") as ListView;
            _SecondaryListView = GetTemplateChild("SecondaryListView") as ListView;
            _LightDismissLayer = GetTemplateChild("LightDismissLayer") as Rectangle;
            _GestureRectangle = GetTemplateChild("GestureRectangle") as Rectangle;
            _LeftHeaderPresenter = GetTemplateChild("LeftHeaderPresenter") as ContentPresenter;
            _RightHeaderPresenter = GetTemplateChild("RightHeaderPresenter") as ContentPresenter;

            if (_ContentRoot != null)
            {
                ImplicitAnimationHelper.CreateAnimation(ElementCompositionPreview.GetElementVisual(_ContentRoot), "Offset", TimeSpan.FromSeconds(0.2d));
            }
            if (_HeaderBorder != null)
            {
                ImplicitAnimationHelper.CreateAnimation(ElementCompositionPreview.GetElementVisual(_HeaderBorder), "Offset", TimeSpan.FromSeconds(0.2d));
            }

            UpdateDisplayMode(HamburgerViewDisplayMode.Overlay);
            UpdateHeaderLayout();
        }



        private void UpdateDisplayMode(HamburgerViewDisplayMode OldMode)
        {
            if (!_templateApplied) return;
            if (OldMode != DisplayMode)
            {
                if (DisplayMode == HamburgerViewDisplayMode.Overlay)
                {
                    IsPaneOpen = false;
                }
                else
                {
                    IsPaneOpen = true;
                }
            }
            if (IsPaneOpen)
            {
                VisualStateManager.GoToState(this, "Open" + DisplayMode.ToString(), DisplayMode == HamburgerViewDisplayMode.Overlay);
            }
            else
            {
                VisualStateManager.GoToState(this, "Close" + DisplayMode.ToString(), DisplayMode == HamburgerViewDisplayMode.Overlay);
            }
        }

        private void UpdatePaneOpenState()
        {
            if (!_templateApplied) return;
            if (IsPaneOpen)
            {
                VisualStateManager.GoToState(this, "Open" + DisplayMode.ToString(), DisplayMode == HamburgerViewDisplayMode.Overlay);
            }
            else
            {
                VisualStateManager.GoToState(this, "Close" + DisplayMode.ToString(), DisplayMode == HamburgerViewDisplayMode.Overlay);
                if (DisplayMode == HamburgerViewDisplayMode.Overlay && _HasManipulated)
                {
                    _PaneRoot.Visibility = Visibility.Collapsed;
                    _PaneRootTransform.TranslateX = -240;
                    _HasManipulated = false;
                }
            }
        }

        private void UpdateHeaderLayout()
        {
            string Visibility = (BackButtonVisibility == Windows.UI.Xaml.Visibility.Visible ? "Show" : "Hide") + "BackButton";
            string IsFull = IsFullScreen ? "FullScreen" : "";
            if (DisplayMode == HamburgerViewDisplayMode.Overlay)
            {
                VisualStateManager.GoToState(this, Visibility + "Close" + IsFull, false);
            }
            else
            {
                if (IsPaneOpen)
                {
                    VisualStateManager.GoToState(this, Visibility + "Close" + IsFull, false);
                    VisualStateManager.GoToState(this, Visibility + "Open" + IsFull, false);
                }
                else
                {
                    VisualStateManager.GoToState(this, Visibility + "Open" + IsFull, false);
                    VisualStateManager.GoToState(this, Visibility + "Close" + IsFull, false);
                }
            }
        }


        private ListView SetSeletcedItem(object Item)
        {
            _PrimaryListView.SelectedItem = Item;
            if (_PrimaryListView.SelectedItem != Item)
            {
                _PrimaryListView.SelectedItem = null;
                _SecondaryListView.SelectedItem = Item;
                return _SecondaryListView;
            }
            _SecondaryListView.SelectedItem = Item;
            if (_SecondaryListView.SelectedItem != Item)
            {
                _SecondaryListView.SelectedItem = null;
                _PrimaryListView.SelectedItem = Item;
                return _PrimaryListView;
            }
            return null;
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackButtonClick();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            IsPaneOpen = !IsPaneOpen;
        }


        private void _ItemClick(object sender, ItemClickEventArgs e)
        {
            _IsSelectedChangedByClick = true;
            SelectedItem = e.ClickedItem;
        }

        private void _NewListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tmpListView = ((ListView)sender);
            tmpListView.SelectionChanged -= _NewListView_SelectionChanged;
            tmpListView.SelectedItem = e.RemovedItems.FirstOrDefault();
        }

        private void _LightDismissLayer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsPaneOpen = false;
        }


        private void _GestureRectangle_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            //_PaneRoot.Visibility = Visibility.Visible;
            VisualStateManager.GoToState(this, "OverlayManipulation", true);
            _TransDelta = -240;
        }

        private void _GestureRectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var tmp = _TransDelta + e.Delta.Translation.X;
            if (tmp > -240 && tmp < 0)
            {
                _TransDelta += e.Delta.Translation.X;
                _PaneRootTransform.TranslateX = _TransDelta;
            }
        }

        private void _GestureRectangle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (_TransDelta <= -120)
            {
                VisualStateManager.GoToState(this, "CloseOverlay", true);
            }
            else
            {
                IsPaneOpen = true;
            }
            _TransDelta = 0;
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public Brush InlinePaneBackground
        {
            get { return (Brush)GetValue(InlinePaneBackgroundProperty); }
            set { SetValue(InlinePaneBackgroundProperty, value); }
        }

        public Brush OverlayPaneBackground
        {
            get { return (Brush)GetValue(OverlayPaneBackgroundProperty); }
            set { SetValue(OverlayPaneBackgroundProperty, value); }
        }

        public HamburgerViewDisplayMode DisplayMode
        {
            get { return (HamburgerViewDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public object PaneHeader
        {
            get { return (object)GetValue(PaneHeaderProperty); }
            set { SetValue(PaneHeaderProperty, value); }
        }

        public object PrimaryItemsSource
        {
            get { return (object)GetValue(PrimaryItemsSourceProperty); }
            set { SetValue(PrimaryItemsSourceProperty, value); }
        }

        public object SecondaryItemsSource
        {
            get { return (object)GetValue(SecondaryItemsSourceProperty); }
            set { SetValue(SecondaryItemsSourceProperty, value); }
        }

        public bool IsPaneOpen
        {
            get { return (bool)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
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

        public Visibility HeaderVisibility
        {
            get { return (Visibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        public bool IsBackButtonEnable
        {
            get { return (bool)GetValue(IsBackButtonEnableProperty); }
            set { SetValue(IsBackButtonEnableProperty, value); }
        }

        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        public bool IsFullScreen
        {
            get { return (bool)GetValue(IsFullScreenProperty); }
            set { SetValue(IsFullScreenProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty InlinePaneBackgroundProperty =
            DependencyProperty.Register("InlinePaneBackground", typeof(Brush), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty OverlayPaneBackgroundProperty =
            DependencyProperty.Register("OverlayPaneBackground", typeof(Brush), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(HamburgerViewDisplayMode), typeof(HamburgerView), new PropertyMetadata(HamburgerViewDisplayMode.Overlay, DisplayModePropertyChanged));

        public static readonly DependencyProperty PaneLengthProperty =
            DependencyProperty.Register("PaneLength", typeof(double), typeof(HamburgerView), new PropertyMetadata(0d));

        public static readonly DependencyProperty PaneHeaderProperty =
            DependencyProperty.Register("PaneHeader", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty PrimaryItemsSourceProperty =
            DependencyProperty.Register("PrimaryItemsSource", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty SecondaryItemsSourceProperty =
            DependencyProperty.Register("SecondaryItemsSource", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(HamburgerView), new PropertyMetadata(false, IsPaneOpenPropertyChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(HamburgerView), new PropertyMetadata(null, SelectedItemPropertyChanged));

        public static readonly DependencyProperty LeftHeaderProperty =
            DependencyProperty.Register("LeftHeader", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty RightHeaderProperty =
            DependencyProperty.Register("RightHeader", typeof(object), typeof(HamburgerView), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftHeaderTemplateProperty =
            DependencyProperty.Register("LeftHeaderTemplate", typeof(DataTemplate), typeof(HamburgerView), new PropertyMetadata(new DataTemplate()));

        public static readonly DependencyProperty RightHeaderTemplateProperty =
            DependencyProperty.Register("RightHeaderTemplate", typeof(DataTemplate), typeof(HamburgerView), new PropertyMetadata(new DataTemplate()));

        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(Visibility), typeof(HamburgerView), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty IsBackButtonEnableProperty =
            DependencyProperty.Register("IsBackButtonEnable", typeof(bool), typeof(HamburgerView), new PropertyMetadata(false));

        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(HamburgerView), new PropertyMetadata(Visibility.Visible, BackButtonVisibilityPropertyChanged));

        public static readonly DependencyProperty IsFullScreenProperty =
            DependencyProperty.Register("IsFullScreen", typeof(bool), typeof(HamburgerView), new PropertyMetadata(false, IsFullScreenPropertyChanged));

        private static void DisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var NewMode = (HamburgerViewDisplayMode)e.NewValue;
            var OldMode = (HamburgerViewDisplayMode)e.OldValue;
            if (NewMode != OldMode)
            {
                if (d is HamburgerView sender)
                {
                    sender.UpdateDisplayMode(OldMode);
                    sender.OnDisplayModeChanged();
                    sender.UpdateHeaderLayout();
                }
            }
        }


        private static void IsPaneOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is HamburgerView sender)
                {
                    sender.UpdatePaneOpenState();
                    sender.UpdateHeaderLayout();
                }
            }
        }

        private static void SelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is HamburgerView sender)
                {
                    if (sender._HasCanceled) return;
                    var SelectedListView = sender.SetSeletcedItem(e.NewValue);
                    if (SelectedListView != null)
                    {
                        if (sender._IsSelectedChangedByClick)
                        {
                            sender.OnItemClick(e.NewValue);
                            sender._IsSelectedChangedByClick = false;
                        }
                        var Cancel = sender.OnSelectionChanging(e.NewValue);

                        if (Cancel)
                        {
                            sender.SetSeletcedItem(e.OldValue);
                            sender._HasCanceled = true;
                            sender.SelectedItem = e.OldValue;
                            sender._HasCanceled = false;
                            SelectedListView.SelectionChanged += sender._ItemsSelectedChangedHandler;
                        }
                        else
                        {
                            if (sender.DisplayMode == HamburgerViewDisplayMode.Inline && sender.IsPaneOpen)
                            {
                                var OldCantainer = sender._PrimaryListView.ContainerFromItem(e.OldValue) ?? sender._SecondaryListView.ContainerFromItem(e.OldValue);
                                var NewCantainer = sender._PrimaryListView.ContainerFromItem(e.NewValue) ?? sender._SecondaryListView.ContainerFromItem(e.NewValue);

                                var OldRectangle = OldCantainer?.VisualTreeFindName("FocusRectangle") as UIElement;
                                var NewRectangle = NewCantainer?.VisualTreeFindName("FocusRectangle") as UIElement;

                                if (OldRectangle != null && NewRectangle != null)
                                {
                                    var connectedAnimation = ConnectedAnimationService
                                        .GetForCurrentView()
                                        .PrepareToAnimate("FocusRectangle", OldRectangle)
                                        .TryStart(NewRectangle);
                                }
                            }
                            else
                            {
                                sender.IsPaneOpen = false;
                            }
                        }

                    }
                }
            }
        }

        private static void BackButtonVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue != (int)e.OldValue)
            {
                if (d is HamburgerView sender)
                {
                    sender.UpdateHeaderLayout();
                }
            }
        }

        private static void IsFullScreenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is HamburgerView sender)
                {
                    sender.UpdateHeaderLayout();
                }
            }
        }


        public event EventHandler DisplayModeChanged;
        public event EventHandler<HamburgerViewItemClickEventArgs> ItemClick;
        public event EventHandler<HamburgerViewSelectionChangingEventArgs> SelectionChanging;
        public event RoutedEventHandler BackButtonClick;

        private void OnDisplayModeChanged()
        {
            DisplayModeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnItemClick(object Item)
        {
            var args = new HamburgerViewItemClickEventArgs(Item);
            ItemClick?.Invoke(this, args);
        }

        private bool OnSelectionChanging(object Item)
        {
            var args = new HamburgerViewSelectionChangingEventArgs(Item);
            SelectionChanging?.Invoke(this, args);
            return args.CancelSelection;
        }

        private void OnBackButtonClick()
        {
            var arg = new RoutedEventArgs();
            BackButtonClick?.Invoke(this, arg);
        }
    }

    public enum HamburgerViewDisplayMode
    {
        Overlay = 0,
        Inline = 1,
    }

    public class HamburgerViewSelectionChangingEventArgs
    {
        internal HamburgerViewSelectionChangingEventArgs(object Item)
        {
            this.SelectedItem = Item;
        }

        public object SelectedItem { get; }
        public bool CancelSelection { get; set; } = false;
    }

    public class HamburgerViewItemClickEventArgs
    {
        internal HamburgerViewItemClickEventArgs(object Item)
        {
            this.ClickedItem = Item;
        }

        public object ClickedItem { get; }
    }
}
