using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Sample.Factorys
{
    public class AppTitleBarFactory
    {
        public AppTitleBarFactory(FrameworkElement customTitleBar, ElementTheme theme = ElementTheme.Light)
        {
            CustomTitleBar = customTitleBar;
            Theme = theme;
            CoreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
            CoreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            SetTitleBar(true);
        }

        ~AppTitleBarFactory()
        {
            CoreTitleBar.IsVisibleChanged -= CoreTitleBar_IsVisibleChanged;
            CoreTitleBar.LayoutMetricsChanged -= CoreTitleBar_LayoutMetricsChanged;
            SetTitleBar(false);
        }

        private ElementTheme? _Theme;

        public FrameworkElement CustomTitleBar { get; set; }
        public CoreApplicationViewTitleBar CoreTitleBar => CoreApplication.GetCurrentView().TitleBar;
        public ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
        public ElementTheme? Theme
        {
            get => _Theme;
            set
            {
                _Theme = value;
                if (value.HasValue)
                {
                    UpdateTitleBarTheme();
                }
                else
                {
                    DisableCustomTitleBarTheme();
                }
            }
        }

        public void SetTitleBar(bool enable)
        {
            if (enable)
            {
                CoreTitleBar.ExtendViewIntoTitleBar = true;
                Window.Current.SetTitleBar(CustomTitleBar);
                UpdateTitleBarTheme();
            }
            else
            {
                CoreTitleBar.ExtendViewIntoTitleBar = false;
                Window.Current.SetTitleBar(null);
                DisableCustomTitleBarTheme();
            }
        }

        private void UpdateTitleBarTheme()
        {
            var foregroundColor = Theme == ElementTheme.Light ? Colors.Black : Colors.White;
            var inactiveBackgroundColor = Theme == ElementTheme.Light ? Colors.White : Colors.Black;
            var hoverBackgroundColor = Theme == ElementTheme.Light ? Colors.LightGray : Colors.DarkGray;
            var pressedBackgroundColor = Colors.Gray;
            var titleBar = TitleBar;

            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = foregroundColor;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverForegroundColor = foregroundColor;
            titleBar.ButtonHoverBackgroundColor = hoverBackgroundColor;
            titleBar.ButtonPressedForegroundColor = foregroundColor;
            titleBar.ButtonPressedBackgroundColor = pressedBackgroundColor;

            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Colors.Gray;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void DisableCustomTitleBarTheme()
        {
            var titleBar = TitleBar;
            titleBar.ForegroundColor = null;
            titleBar.BackgroundColor = null;
            titleBar.ButtonForegroundColor = null;
            titleBar.ButtonBackgroundColor = null;
            titleBar.ButtonHoverForegroundColor = null;
            titleBar.ButtonHoverBackgroundColor = null;
            titleBar.ButtonPressedForegroundColor = null;
            titleBar.ButtonPressedBackgroundColor = null;

            titleBar.InactiveForegroundColor = null;
            titleBar.InactiveBackgroundColor = null;
            titleBar.ButtonInactiveForegroundColor = null;
            titleBar.ButtonInactiveBackgroundColor = null;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (CustomTitleBar == null) return;
            CustomTitleBar.Height = sender.Height;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (CustomTitleBar == null) return;
            if (sender.IsVisible)
            {
                CustomTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                CustomTitleBar.Visibility = Visibility.Collapsed;
            }

        }
    }
}
