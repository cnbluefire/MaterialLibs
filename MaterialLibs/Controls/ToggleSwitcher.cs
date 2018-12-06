using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls
{
    public sealed class ToggleSwitcher : Control
    {
        public ToggleSwitcher()
        {
            this.DefaultStyleKey = typeof(ToggleSwitcher);
            this.SizeChanged += ToggleSwitcher_SizeChanged;
        }

        Grid RootGrid;
        DivideView ContentView;
        Border LeftBorder;
        Border RightBorder;
        Grid SelectedPieRoot;
        Rectangle PieShadowHost;
        Rectangle SelectedPie;
        Rectangle BackgroundRectangle;
        DivideView SelectedContentView;
        Border LeftSelectedBorder;
        Border RightSelectedBorder;
        RectangleGeometry SelectedContentViewClip;
        TranslateTransform SelectedPieTrans;
        TranslateTransform SelectedContentViewClipTrans;

        Storyboard SelectedPieToLeftStoryboard;
        Storyboard SelectedPieToRightStoryboard;
        Storyboard SelectedContentGridClipToLeftStoryboard;
        Storyboard SelectedContentGridClipToRightStoryboard;

        DoubleAnimation SelectedPieToLeftAnimation;
        DoubleAnimation SelectedPieToRightAnimation;
        DoubleAnimation SelectedContentGridClipToLeftAnimation;
        DoubleAnimation SelectedContentGridClipToRightAnimation;


        double Radius;
        double PieMaxTranslationX;
        double LastDelta;
        bool IsTemplateApplied;
        bool IsCoreChange;

        SpriteVisual ShadowVisual;
        DropShadow Shadow;
        ExpressionAnimation SizeBind;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootGrid = GetTemplateChild("RootGrid") as Grid;
            SelectedPieRoot = GetTemplateChild("SelectedPieRoot") as Grid;
            BackgroundRectangle = GetTemplateChild("BackgroundRectangle") as Rectangle;
            PieShadowHost = GetTemplateChild("PieShadowHost") as Rectangle;
            SelectedPie = GetTemplateChild("SelectedPie") as Rectangle;

            ContentView = GetTemplateChild("ContentView") as DivideView;
            LeftBorder = GetTemplateChild("LeftBorder") as Border;
            RightBorder = GetTemplateChild("RightBorder") as Border;

            SelectedContentView = GetTemplateChild("SelectedContentView") as DivideView;
            LeftSelectedBorder = GetTemplateChild("LeftSelectedBorder") as Border;
            RightSelectedBorder = GetTemplateChild("RightSelectedBorder") as Border;

            SelectedContentViewClip = GetTemplateChild("SelectedContentViewClip") as RectangleGeometry;
            SelectedPieTrans = GetTemplateChild("SelectedPieTrans") as TranslateTransform;
            SelectedContentViewClipTrans = GetTemplateChild("SelectedContentViewClipTrans") as TranslateTransform;

            if (ContentView != null)
            {
                ContentView.Tapped += ContentGrid_Tapped;
            }

            UpdateSize();
            InitShadow();
            InitAnimation();

            IsTemplateApplied = true;
        }

        protected override void OnManipulationStarted(ManipulationStartedRoutedEventArgs e)
        {
            base.OnManipulationStarted(e);
            if (SelectedPieTrans != null)
            {
                LastDelta = SelectedPieTrans.X;
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            base.OnManipulationDelta(e);
            if (SelectedPieTrans != null && SelectedContentViewClipTrans != null)
            {
                var trans = LastDelta + e.Cumulative.Translation.X;

                if (trans > 0 && trans < PieMaxTranslationX)
                {
                    SelectedPieTrans.X = trans;
                    SelectedContentViewClipTrans.X = trans;
                }
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedRoutedEventArgs e)
        {
            base.OnManipulationCompleted(e);
            Judge(Math.Abs(e.Cumulative.Translation.X) < this.ActualWidth / 20);
            LastDelta = 0;
        }

        private void ContentGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            if (State == ToggleSwitcherState.Left)
            {
                State = ToggleSwitcherState.Right;
            }
            else
            {
                State = ToggleSwitcherState.Left;
            }
        }

        private void InitShadow()
        {
            if (SelectedPie != null && PieShadowHost != null)
            {
                var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
                SizeBind = compositor.CreateExpressionAnimation("host.Size");
                SizeBind.SetReferenceParameter("host", ElementCompositionPreview.GetElementVisual(PieShadowHost));
                ShadowVisual = compositor.CreateSpriteVisual();
                ShadowVisual.StartAnimation("Size", SizeBind);

                Shadow = compositor.CreateDropShadow();
                Shadow.Color = Colors.Black;
                Shadow.Opacity = 0.8f;
                Shadow.BlurRadius = 6f;
                Shadow.Mask = SelectedPie.GetAlphaMask();

                ShadowVisual.Shadow = Shadow;

                if (IsShadowEnable)
                {
                    if (ElementCompositionPreview.GetElementChildVisual(PieShadowHost) == null)
                    {
                        ElementCompositionPreview.SetElementChildVisual(PieShadowHost, ShadowVisual);
                    }
                }
            }
        }

        private void InitAnimation()
        {
            SelectedPieToLeftStoryboard = new Storyboard();
            SelectedPieToRightStoryboard = new Storyboard();
            SelectedContentGridClipToLeftStoryboard = new Storyboard();
            SelectedContentGridClipToRightStoryboard = new Storyboard();

            var easingFunc = new CubicEase() { EasingMode = EasingMode.EaseOut };
            var Duration = TimeSpan.FromSeconds(0.2d);

            SelectedPieToLeftAnimation = new DoubleAnimation() { EasingFunction = easingFunc, To = 0, Duration = Duration };
            SelectedPieToRightAnimation = new DoubleAnimation() { EasingFunction = easingFunc, To = PieMaxTranslationX, Duration = Duration };
            SelectedContentGridClipToLeftAnimation = new DoubleAnimation() { EasingFunction = easingFunc, To = 0, Duration = Duration };
            SelectedContentGridClipToRightAnimation = new DoubleAnimation() { EasingFunction = easingFunc, To = PieMaxTranslationX, Duration = Duration };

            Storyboard.SetTarget(SelectedPieToLeftAnimation, SelectedPieTrans);
            Storyboard.SetTarget(SelectedPieToRightAnimation, SelectedPieTrans);
            Storyboard.SetTarget(SelectedContentGridClipToLeftAnimation, SelectedContentViewClipTrans);
            Storyboard.SetTarget(SelectedContentGridClipToRightAnimation, SelectedContentViewClipTrans);

            Storyboard.SetTargetProperty(SelectedPieToLeftAnimation, "X");
            Storyboard.SetTargetProperty(SelectedPieToRightAnimation, "X");
            Storyboard.SetTargetProperty(SelectedContentGridClipToLeftAnimation, "X");
            Storyboard.SetTargetProperty(SelectedContentGridClipToRightAnimation, "X");

            SelectedPieToLeftStoryboard.Children.Add(SelectedPieToLeftAnimation);
            SelectedPieToRightStoryboard.Children.Add(SelectedPieToRightAnimation);
            SelectedContentGridClipToLeftStoryboard.Children.Add(SelectedContentGridClipToLeftAnimation);
            SelectedContentGridClipToRightStoryboard.Children.Add(SelectedContentGridClipToRightAnimation);
        }

        private void UpdateSize()
        {
            Radius = this.ActualHeight / 2;
            if (BackgroundRectangle != null)
            {
                BackgroundRectangle.RadiusX = Radius;
                BackgroundRectangle.RadiusY = Radius;
            }
            if (SelectedPieRoot != null)
            {
                SelectedPieRoot.Width = (this.ActualWidth / 2);
            }
            if (SelectedPie != null)
            {
                SelectedPie.RadiusX = Radius;
                SelectedPie.RadiusY = Radius;
                PieMaxTranslationX = (this.ActualWidth / 2);

                if (Shadow != null)
                {
                    Shadow.Mask = SelectedPie.GetAlphaMask();
                }
            }
            if (SelectedContentViewClip != null)
            {
                SelectedContentViewClip.Rect = new Windows.Foundation.Rect(0, 0, this.ActualWidth / 2, this.ActualHeight);
            }
            if (SelectedPieToRightAnimation != null)
            {
                SelectedPieToRightAnimation.To = PieMaxTranslationX;
            }
            if (SelectedContentGridClipToRightAnimation != null)
            {
                SelectedContentGridClipToRightAnimation.To = PieMaxTranslationX;
            }
            if (State == ToggleSwitcherState.Right)
            {
                if (SelectedPieTrans != null)
                {
                    SelectedPieTrans.X = PieMaxTranslationX;
                }
                if (SelectedContentViewClipTrans != null)
                {
                    SelectedContentViewClipTrans.X = PieMaxTranslationX;
                }
            }
        }

        private void Judge(bool IsTap)
        {
            if (SelectedPieTrans != null)
            {
                if (IsTap)
                {
                    if (State == ToggleSwitcherState.Left)
                    {
                        SelectedPieToLeftStoryboard.Begin();
                        SelectedContentGridClipToLeftStoryboard.Begin();
                    }
                    else
                    {
                        SelectedPieToRightStoryboard.Begin();
                        SelectedContentGridClipToRightStoryboard.Begin();
                    }
                }
                else
                {
                    IsCoreChange = true;
                    if (SelectedPieTrans.X > PieMaxTranslationX / 2)
                    {
                        SelectedPieToRightStoryboard.Begin();
                        SelectedContentGridClipToRightStoryboard.Begin();
                        State = ToggleSwitcherState.Right;
                    }
                    else
                    {
                        SelectedPieToLeftStoryboard.Begin();
                        SelectedContentGridClipToLeftStoryboard.Begin();
                        State = ToggleSwitcherState.Left;
                    }
                    IsCoreChange = false;
                }
            }
        }

        private void ToggleSwitcher_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize();
        }

        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }

        public DataTemplate LeftTemplate
        {
            get { return (DataTemplate)GetValue(LeftTemplateProperty); }
            set { SetValue(LeftTemplateProperty, value); }
        }

        public DataTemplate RightTemplate
        {
            get { return (DataTemplate)GetValue(RightTemplateProperty); }
            set { SetValue(RightTemplateProperty, value); }
        }

        public object LeftContent
        {
            get { return (object)GetValue(LeftContentProperty); }
            set { SetValue(LeftContentProperty, value); }
        }

        public object RightContent
        {
            get { return (object)GetValue(RightContentProperty); }
            set { SetValue(RightContentProperty, value); }
        }

        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }

        public ToggleSwitcherState State
        {
            get { return (ToggleSwitcherState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public bool IsShadowEnable
        {
            get { return (bool)GetValue(IsShadowEnableProperty); }
            set { SetValue(IsShadowEnableProperty, value); }
        }

        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftTemplateProperty =
            DependencyProperty.Register("LeftTemplate", typeof(DataTemplate), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty RightTemplateProperty =
            DependencyProperty.Register("RightTemplate", typeof(DataTemplate), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register("LeftContent", typeof(object), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register("RightContent", typeof(object), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(ToggleSwitcher), new PropertyMetadata(null));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(ToggleSwitcherState), typeof(ToggleSwitcher), new PropertyMetadata(ToggleSwitcherState.Left, (s, a) =>
            {
                if ((ToggleSwitcherState)a.NewValue != (ToggleSwitcherState)a.OldValue)
                {
                    if (s is ToggleSwitcher sender)
                    {
                        if (sender.IsTemplateApplied)
                        {
                            if (!sender.IsCoreChange)
                            {
                                switch ((ToggleSwitcherState)a.NewValue)
                                {
                                    case ToggleSwitcherState.Left:
                                        sender.SelectedPieToLeftStoryboard?.Begin();
                                        sender.SelectedContentGridClipToLeftStoryboard?.Begin();
                                        break;
                                    case ToggleSwitcherState.Right:
                                        sender.SelectedPieToRightStoryboard?.Begin();
                                        sender.SelectedContentGridClipToRightStoryboard?.Begin();
                                        break;
                                }
                            }
                            sender.OnStateChanged();
                        }
                    }
                }
            }));

        public static readonly DependencyProperty IsShadowEnableProperty =
            DependencyProperty.Register("IsShadowEnable", typeof(bool), typeof(ToggleSwitcher), new PropertyMetadata(true, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is ToggleSwitcher sender)
                    {
                        if (sender.IsTemplateApplied)
                        {
                            if (sender.PieShadowHost != null && sender.ShadowVisual != null)
                            {
                                if ((bool)a.NewValue)
                                {
                                    if (ElementCompositionPreview.GetElementChildVisual(sender.PieShadowHost) == null)
                                    {
                                        ElementCompositionPreview.SetElementChildVisual(sender.PieShadowHost, sender.ShadowVisual);
                                    }
                                }
                                else
                                {
                                    ElementCompositionPreview.SetElementChildVisual(sender.PieShadowHost, null);
                                }
                            }
                        }
                    }
                }
            }));




        public event StateChangedEventHandler StateChanged;
        private void OnStateChanged()
        {
            StateChanged?.Invoke(this, new StateChangedEventArgs(State));
        }
    }

    public enum ToggleSwitcherState
    {
        Left, Right
    }

    public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

    public class StateChangedEventArgs : EventArgs
    {
        public StateChangedEventArgs(ToggleSwitcherState state)
        {
            State = state;
        }

        public ToggleSwitcherState State { get; set; }
    }
}
