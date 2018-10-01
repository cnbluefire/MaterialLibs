using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls
{
    [ContentProperty(Name = "Content")]
    public sealed class CardView : Control, IInteractionTrackerOwner
    {
        public CardView()
        {
            this.DefaultStyleKey = typeof(CardView);
            this.Loaded += _Loaded;
        }

        //private Grid ContentGrid;
        private Grid RootGrid;
        private Border ContentBorder;
        private Rectangle ShadowHost;
        private Rectangle LightDismissLayer;
        private Rectangle BorderBackground;
        private Button CloseButton;

        private Compositor Compositor;
        private Visual BorderBackgroundVisual;
        private Visual ContentBorderVisual;
        private Visual LightDismissLayerVisual;
        private SpriteVisual ShadowVisual;
        private InteractionTracker m_tracker;
        private VisualInteractionSource m_source;
        private InteractionTrackerInertiaRestingValue m_open;
        private InteractionTrackerInertiaRestingValue m_close;
        private ExpressionAnimation ContentOffsetExp;
        private ExpressionAnimation LightDismissLayerOpacityExp;
        private Vector3KeyFrameAnimation OpenAnimation;
        private Vector3KeyFrameAnimation CloseAnimation;
        private Vector3KeyFrameAnimation ToOpenAnimation;

        private bool _IsLoaded;
        private bool _IsJudging;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //ContentGrid = GetTemplateChild("ContentGrid") as Grid;
            RootGrid = GetTemplateChild("RootGrid") as Grid;
            ContentBorder = GetTemplateChild("ContentBorder") as Border;
            ShadowHost = GetTemplateChild("ShadowHost") as Rectangle;
            LightDismissLayer = GetTemplateChild("LightDismissLayer") as Rectangle;
            BorderBackground = GetTemplateChild("BorderBackground") as Rectangle;
            CloseButton = GetTemplateChild("CloseButton") as Button;

            if (ContentBorder != null)
            {
                ContentBorder.SizeChanged += ContentBorder_SizeChanged;
                ContentBorder.AddHandler(PointerPressedEvent, new PointerEventHandler(ContentBorder_PointerPressed), true);
                ContentBorder.AddHandler(PointerReleasedEvent, new PointerEventHandler(ContentBorder_PointerReleased), true);
                ContentBorder.AddHandler(PointerCanceledEvent, new PointerEventHandler(ContentBorder_PointerCanceled), true);
            }
            if (LightDismissLayer != null)
            {
                LightDismissLayer.Tapped += LightDismissLayer_Tapped;
                LightDismissLayerVisual = ElementCompositionPreview.GetElementVisual(LightDismissLayer);
            }
            if (CloseButton != null)
            {
                CloseButton.Click += CloseButton_Click;
            }
            UpdateIsOpen();
        }

        private void _Loaded(object sender, RoutedEventArgs e)
        {
            SetupCompositor();
            SetupInteraction();
            _IsLoaded = true;
        }

        private void ContentBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_tracker != null)
            {
                var height = (float)e.NewSize.Height;
                m_tracker.MaxPosition = new Vector3(0f, 0f, 0f);
                m_tracker.MinPosition = new Vector3(0f, -height, 0f);
            }
            if (ShadowVisual != null)
            {
                if (BorderBackground != null)
                {
                    if (ShadowVisual.Shadow is DropShadow shadow)
                    {
                        shadow.Mask = BorderBackground.GetAlphaMask();
                    }
                }
                ShadowVisual.Size = new Vector2((float)ShadowHost.ActualWidth, (float)ShadowHost.ActualHeight);
            }
        }

        private void ContentBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (IsRedirectForManipulationEnable)
            {
                if (m_source != null)
                {
                    if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                    {
                        //Mobile RengeBase导致VisualInteractionSource.TryRedirectForManipulation出错
                        //if (e.OriginalSource is Rectangle rect && rect.Name.Contains("TrackRect"))
                        //{
                        //    return;
                        //}
                        if (ContentBorder.CapturePointer(e.Pointer))
                        {
                            ContentBorder.ReleasePointerCapture(e.Pointer);
                            m_source.TryRedirectForManipulation(e.GetCurrentPoint(ContentBorder));
                        }
                    }
                }
            }
        }

        private void ContentBorder_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ContentBorder.ReleasePointerCapture(e.Pointer);
        }

        private void ContentBorder_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            ContentBorder.ReleasePointerCapture(e.Pointer);
        }

        private void LightDismissLayer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void SetupCompositor()
        {
            if (DesignMode.DesignModeEnabled) return;
            Compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            if (BorderBackground != null)
            {
                BorderBackgroundVisual = ElementCompositionPreview.GetElementVisual(BorderBackground);
            }
            if (ContentBorder != null)
            {
                ElementCompositionPreview.SetIsTranslationEnabled(ContentBorder, true);
                ContentBorderVisual = ElementCompositionPreview.GetElementVisual(ContentBorder);
            }
            if (ShadowHost != null)
            {
                var shadow = Compositor.CreateDropShadow();
                shadow.Color = Colors.Black;
                shadow.Opacity = 0.4f;
                shadow.BlurRadius = 8f;
                shadow.Offset = new Vector3(2f, 2f, 0f);
                if (BorderBackground != null)
                {
                    shadow.Mask = BorderBackground.GetAlphaMask();
                }
                ShadowVisual = Compositor.CreateSpriteVisual();
                ShadowVisual.Size = new Vector2((float)ShadowHost.ActualWidth, (float)ShadowHost.ActualHeight);
                ShadowVisual.Shadow = shadow;
                ElementCompositionPreview.SetElementChildVisual(ShadowHost, ShadowVisual);
            }
        }

        private void SetupInteraction()
        {
            if (DesignMode.DesignModeEnabled) return;
            if (Compositor == null) return;
            if (ContentBorderVisual == null) return;
            m_tracker = InteractionTracker.CreateWithOwner(Compositor, this);
            var height = (float)ContentBorder.ActualHeight;
            m_tracker.MaxPosition = new Vector3(0f, 0f, 0f);
            m_tracker.MinPosition = new Vector3(0f, -height, 0f);

            m_source = VisualInteractionSource.Create(ContentBorderVisual);
            m_source.IsPositionYRailsEnabled = true;
            m_source.ManipulationRedirectionMode = VisualInteractionSourceRedirectionMode.CapableTouchpadOnly;
            m_source.PositionYChainingMode = InteractionChainingMode.Auto;
            m_source.PositionYSourceMode = InteractionSourceMode.EnabledWithoutInertia;

            m_tracker.InteractionSources.Add(m_source);

            m_open = InteractionTrackerInertiaRestingValue.Create(Compositor);
            m_open.Condition = Compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.Y > -host.Size.Y / 3");
            m_open.RestingValue = Compositor.CreateExpressionAnimation("0");
            m_open.Condition.SetReferenceParameter("host", ContentBorderVisual);
            m_open.RestingValue.SetReferenceParameter("host", ContentBorderVisual);

            m_close = InteractionTrackerInertiaRestingValue.Create(Compositor);
            m_close.Condition = Compositor.CreateExpressionAnimation("this.Target.NaturalRestingPosition.Y <= -host.Size.Y / 3");
            m_close.RestingValue = Compositor.CreateExpressionAnimation("-host.Size.Y");
            m_close.Condition.SetReferenceParameter("host", ContentBorderVisual);
            m_close.RestingValue.SetReferenceParameter("host", ContentBorderVisual);


            //Release模式会爆炸
            //var modifiers = new InteractionTrackerInertiaRestingValue[] { m_open, m_close };
            //m_tracker.ConfigurePositionYInertiaModifiers(modifiers);


            ContentOffsetExp = Compositor.CreateExpressionAnimation("Max(-15f, -tracker.Position.Y)");
            ContentOffsetExp.SetReferenceParameter("tracker", m_tracker);
            ContentBorderVisual.StartAnimation("Translation.Y", ContentOffsetExp);

            LightDismissLayerOpacityExp = Compositor.CreateExpressionAnimation("Clamp(1 + (tracker.Position.Y / host.Size.Y),0,1)");
            LightDismissLayerOpacityExp.SetReferenceParameter("tracker", m_tracker);
            LightDismissLayerOpacityExp.SetReferenceParameter("host", ContentBorderVisual);
            LightDismissLayerVisual.StartAnimation("Opacity", LightDismissLayerOpacityExp);

            OpenAnimation = Compositor.CreateVector3KeyFrameAnimation();
            OpenAnimation.InsertExpressionKeyFrame(0f, "Vector3(0f, -host.Size.Y, 0f)");
            OpenAnimation.InsertKeyFrame(1f, new Vector3(0f, 15f, 0f));
            OpenAnimation.SetReferenceParameter("host", ContentBorderVisual);
            OpenAnimation.Duration = TimeSpan.FromSeconds(0.3d);

            ToOpenAnimation = Compositor.CreateVector3KeyFrameAnimation();
            ToOpenAnimation.InsertKeyFrame(1f, new Vector3(0f, 5f, 0f));
            ToOpenAnimation.SetReferenceParameter("host", ContentBorderVisual);
            ToOpenAnimation.Duration = TimeSpan.FromSeconds(0.3d);

            CloseAnimation = Compositor.CreateVector3KeyFrameAnimation();
            CloseAnimation.InsertExpressionKeyFrame(1f, "Vector3(0f, -host.Size.Y, 0f)");
            CloseAnimation.SetReferenceParameter("host", ContentBorderVisual);
            CloseAnimation.Duration = TimeSpan.FromSeconds(0.3d);
        }

        private void UpdateIsOpen()
        {
            if (IsOpen)
            {
                VisualStateManager.GoToState(this, "IsOpen", false);

                if (m_tracker != null)
                {
                    m_tracker.TryUpdatePositionWithAnimation(OpenAnimation);
                    //m_tracker.TryUpdatePositionWithAdditionalVelocity(new Vector3(0f,1000f, 0f));

                }
            }
            else
            {
                if (m_tracker != null)
                {
                    m_tracker.TryUpdatePositionWithAnimation(CloseAnimation);
                    //m_tracker.TryUpdatePositionWithAdditionalVelocity(new Vector3(0f,-1000f, 0f));
                }
                else
                {
                    VisualStateManager.GoToState(this, "IsNotOpen", false);
                }
            }
        }

        private void JudgeIsOpen()
        {
            if (_IsLoaded)
            {
                if (m_tracker != null)
                {
                    _IsJudging = true;
                    if (m_tracker.Position.Y == 0f)
                    {
                        IsOpen = true;
                        VisualStateManager.GoToState(this, "IsOpen", false);
                    }
                    else if (ContentBorder != null && m_tracker.Position.Y < -ContentBorder.ActualHeight + 0.1d)
                    {
                        IsOpen = false;
                        VisualStateManager.GoToState(this, "IsNotOpen", false);
                    }
                    else
                    {
                        if (ContentBorder != null)
                        {
                            if (m_tracker.Position.Y > -ContentBorder.ActualHeight / 4)
                            {
                                IsOpen = true;
                                m_tracker.TryUpdatePositionWithAnimation(ToOpenAnimation);
                            }
                            else
                            {
                                IsOpen = false;
                                m_tracker.TryUpdatePositionWithAnimation(CloseAnimation);
                            }
                        }
                    }
                    _IsJudging = false;
                }
            }
        }

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public Brush LightDismissLayerBackground
        {
            get { return (Brush)GetValue(LightDismissLayerBackgroundProperty); }
            set { SetValue(LightDismissLayerBackgroundProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public object Footer
        {
            get { return (object)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        public Visibility LightDismissLayerVisibility
        {
            get { return (Visibility)GetValue(LightDismissLayerVisibilityProperty); }
            set { SetValue(LightDismissLayerVisibilityProperty, value); }
        }

        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        public double ContentHeight
        {
            get { return (double)GetValue(ContentHeightProperty); }
            set { SetValue(ContentHeightProperty, value); }
        }

        public double ContentMaxWidth
        {
            get { return (double)GetValue(ContentMaxWidthProperty); }
            set { SetValue(ContentMaxWidthProperty, value); }
        }

        public double ContentMaxHeight
        {
            get { return (double)GetValue(ContentMaxHeightProperty); }
            set { SetValue(ContentMaxHeightProperty, value); }
        }

        public double ContentMinWidth
        {
            get { return (double)GetValue(ContentMinWidthProperty); }
            set { SetValue(ContentMinWidthProperty, value); }
        }

        public double ContentMinHeight
        {
            get { return (double)GetValue(ContentMinHeightProperty); }
            set { SetValue(ContentMinHeightProperty, value); }
        }

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public bool IsRedirectForManipulationEnable
        {
            get { return (bool)GetValue(IsRedirectForManipulationEnableProperty); }
            set { SetValue(IsRedirectForManipulationEnableProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(CardView), new PropertyMetadata(0d));

        public static readonly DependencyProperty LightDismissLayerBackgroundProperty =
            DependencyProperty.Register("LightDismissLayerBackground", typeof(Brush), typeof(CardView), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(CardView), new PropertyMetadata(false, (s, a) =>
            {
                if ((bool)a.NewValue != (bool)a.OldValue)
                {
                    if (s is CardView sender)
                    {
                        sender.OnIsOpenChanged();
                        if (sender._IsJudging) return;
                        sender.UpdateIsOpen();
                    }
                }
            }));

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(CardView), new PropertyMetadata(null));
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(CardView), new PropertyMetadata(null));
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(object), typeof(CardView), new PropertyMetadata(null));
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(CardView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty LightDismissLayerVisibilityProperty =
            DependencyProperty.Register("LightDismissLayerVisibility", typeof(Visibility), typeof(CardView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register("ContentWidth", typeof(double), typeof(CardView), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register("ContentHeight", typeof(double), typeof(CardView), new PropertyMetadata(double.NaN));
        public static readonly DependencyProperty ContentMaxWidthProperty =
            DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(CardView), new PropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty ContentMaxHeightProperty =
            DependencyProperty.Register("ContentMaxHeight", typeof(double), typeof(CardView), new PropertyMetadata(double.PositiveInfinity));
        public static readonly DependencyProperty ContentMinWidthProperty =
            DependencyProperty.Register("ContentMinWidth", typeof(double), typeof(CardView), new PropertyMetadata(0));
        public static readonly DependencyProperty ContentMinHeightProperty =
            DependencyProperty.Register("ContentMinHeight", typeof(double), typeof(CardView), new PropertyMetadata(0));
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(CardView), new PropertyMetadata(null));
        public static readonly DependencyProperty IsRedirectForManipulationEnableProperty =
            DependencyProperty.Register("IsRedirectForManipulationEnable", typeof(bool), typeof(CardView), new PropertyMetadata(true));

        public event IsOpenChangedEventHandler IsOpenChanged;
        private void OnIsOpenChanged()
        {
            IsOpenChanged?.Invoke(this, new IsOpenChangedEventArgs(IsOpen));
        }

        #region InteractionEvent
        public void CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args)
        {

        }

        public void IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
        {
            JudgeIsOpen();
        }

        public void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {

        }

        public void InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args)
        {


        }

        public void RequestIgnored(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args)
        {

        }

        public void ValuesChanged(InteractionTracker sender, InteractionTrackerValuesChangedArgs args)
        {

        }
        #endregion
    }

    public delegate void IsOpenChangedEventHandler(object sender, IsOpenChangedEventArgs args);

    public class IsOpenChangedEventArgs : EventArgs
    {
        internal IsOpenChangedEventArgs(bool isOpen)
        {
            IsOpen = isOpen;
        }

        public bool IsOpen { get; set; }
    }
}

