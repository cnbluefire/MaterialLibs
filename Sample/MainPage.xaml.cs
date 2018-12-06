using MaterialLibs.Controls;
using MaterialLibs.Factorys;
using MaterialLibs.Services;
using Sample.Factorys;
using Sample.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Sample
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            ThemeItem = new HamburgerViewItem() { Content = "Theme", Icon = "\uE809" };

            PrimaryList = new ObservableCollection<HamburgerViewItem>()
            {
                new HamburgerViewItem(){Content = "Ripple",Icon = "\uEDA4",Tag = typeof(RipplePage)},
                new HamburgerViewItem(){Content = "Particle",Icon = "\uEBD2",Tag = typeof(ParticlePage)},
                new HamburgerViewItem(){Content = "TipsRectangle",Icon = "\uE71D",Tag = typeof(TipsRectanglePage)},
                new HamburgerViewItem(){Content = "ImplicitAnimation",Icon = "\uF133",Tag = typeof(ImplicitAnimationPage)},
                new HamburgerViewItem(){Content = "Bigbang",Icon = "\uE1A1",Tag = typeof(BigbangPage)},
                new HamburgerViewItem(){Content = "Perspective",Icon = "\uE809",Tag = typeof(PerspectivePage)},
                new HamburgerViewItem(){Content = "CardView",Icon = "\uEC6C",Tag = typeof(CardViewPage)},
                new HamburgerViewItem(){Content = "DraggedBadge",Icon = "\uE783",Tag = typeof(DraggedBadgePage)},
                new HamburgerViewItem(){Content = "SwipeBack",Icon = "\uE111",Tag = typeof(SwipeBackPage)},
                new HamburgerViewItem(){Content = "ChromeFlyout",Icon = "\uE111",Tag = typeof(ChromeFlyoutPage)},
            };

            SecondaryList = new ObservableCollection<HamburgerViewItem>()
            {
                //ThemeItem,
                new HamburgerViewItem(){Content = "About",Icon = "\uED54",Tag = typeof(AboutPage)},
            };

            appTitleBarFactory = new AppTitleBarFactory(AppTitleBar);
            service = new NavigationBloomTransitionService() { CurrentFrame = ContentFrame };
        }

        NavigationBloomTransitionService service;

        ToggleSwitcher ThemeSwitcher;
        AppTitleBarFactory appTitleBarFactory;
        HamburgerViewItem ThemeItem { get; set; }
        public ObservableCollection<HamburgerViewItem> PrimaryList { get; set; }
        public ObservableCollection<HamburgerViewItem> SecondaryList { get; set; }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Theme"))
            {
                var theme = (string)ApplicationData.Current.LocalSettings.Values["Theme"];
                UpdateTheme(theme == "Light");
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["Theme"] = "Light";
                UpdateTheme(true);
            }

            _HamburgerView.SelectedItem = PrimaryList[0];
            //ContentFrame.Navigate(typeof(RipplePage), new SuppressNavigationTransitionInfo());
            await service.NavigateAndBloomFromUIElementAsync(ContentFrame, Windows.UI.Colors.White, typeof(RipplePage));
        }

        private async void _HamburgerView_ItemClick(object sender, MaterialLibs.Controls.HamburgerViewItemClickEventArgs e)
        {
            if (e.ClickedItem == ThemeItem)
            {
                UpdateTheme(RequestedTheme == ElementTheme.Dark);
                ApplicationData.Current.LocalSettings.Values["Theme"] = RequestedTheme.ToString();
            }
            else
            {
                //ContentFrame.Navigate((Type)((HamburgerViewItem)e.ClickedItem).Tag, null, new SuppressNavigationTransitionInfo());
                await service.NavigateAndBloomFromUIElementAsync(ContentFrame, Windows.UI.Colors.White, (Type)((HamburgerViewItem)e.ClickedItem).Tag);
            }
        }

        private void _HamburgerView_SelectionChanging(object sender, MaterialLibs.Controls.HamburgerViewSelectionChangingEventArgs e)
        {
            if (e.SelectedItem == ThemeItem)
            {
                e.CancelSelection = true;
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateBackState();
            _HamburgerView.SelectedItem =
                PrimaryList.FirstOrDefault(x => x.Tag == ContentFrame.SourcePageType) ??
                SecondaryList.FirstOrDefault(x => x.Tag == ContentFrame.SourcePageType);
        }

        private void UpdateTheme(bool Light)
        {
            if (Light)
            {
                RequestedTheme = ElementTheme.Light;
                ThemeItem.Icon = "\uE706";
                if (ThemeSwitcher != null)
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Left;
                }
            }
            else
            {
                RequestedTheme = ElementTheme.Dark;
                ThemeItem.Icon = "\uE708";
                if (ThemeSwitcher != null)
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Right;
                }
            }
            appTitleBarFactory.Theme = RequestedTheme;
        }

        private void UpdateBackState()
        {
            if (ContentFrame.CanGoBack)
            {
                _HamburgerView.IsBackButtonEnable = true;
                _HamburgerView.BackButtonVisibility = Visibility.Visible;
            }
            else
            {
                _HamburgerView.IsBackButtonEnable = false;
                _HamburgerView.BackButtonVisibility = Visibility.Collapsed;
            }
        }

        private void _HamburgerView_BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack(new SuppressNavigationTransitionInfo());
            }
        }

        private void ToggleSwitcher_Loaded(object sender, RoutedEventArgs e)
        {
            ThemeSwitcher = sender as ToggleSwitcher;
            if (ThemeSwitcher != null)
            {
                if (RequestedTheme == ElementTheme.Light)
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Left;
                }
                else
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Right;
                }
            }
        }

        private void ToggleSwitcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.State == ToggleSwitcherState.Left)
            {
                if (RequestedTheme != ElementTheme.Light)
                {
                    UpdateTheme(true);
                }
            }
            else
            {
                if (RequestedTheme != ElementTheme.Dark)
                {
                    UpdateTheme(false);
                }
            }
        }
    }
}
