using MaterialLibs.Controls;
using MaterialLibs.Controls.Tab;
using Sample.Factorys;
using Sample.Views;
using Simple.Models;
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

            appTitleBarFactory = new AppTitleBarFactory(AppTitleBar);
        }

        ToggleSwitcher ThemeSwitcher;
        AppTitleBarFactory appTitleBarFactory;


        ObservableCollection<NavigationModel> _Pages = new ObservableCollection<NavigationModel>()
            {
                new NavigationModel(){PageType = typeof(RipplePage), Title = "Ripple",  },
                new NavigationModel(){PageType = typeof(ParticlePage), Title = "Particle",  },
                new NavigationModel(){PageType = typeof(IndicatorPage), Title = "Indicator",  },
                new NavigationModel(){PageType = typeof(ImplicitAnimationPage), Title = "ImplicitAnimation",  },
                new NavigationModel(){PageType = typeof(BigbangPage), Title = "Bigbang",  },
                new NavigationModel(){PageType = typeof(PerspectivePage), Title = "Perspective",  },
                new NavigationModel(){PageType = typeof(CardViewPage), Title = "CardView",  },
                new NavigationModel(){PageType = typeof(DraggedBadgePage), Title = "DraggedBadge",  },
                new NavigationModel(){PageType = typeof(SwipeBackPage), Title = "SwipeBack",  },
                new NavigationModel(){PageType = typeof(AboutPage), Title = "About",  }
            };


        ObservableCollection<NavigationModel> Pages => _Pages;

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

        }


        private void UpdateTheme(bool Light)
        {
            if (Light)
            {
                RequestedTheme = ElementTheme.Light;
                if (ThemeSwitcher != null)
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Left;
                }
            }
            else
            {
                RequestedTheme = ElementTheme.Dark;
                if (ThemeSwitcher != null)
                {
                    ThemeSwitcher.State = ToggleSwitcherState.Right;
                }
            }
            appTitleBarFactory.Theme = RequestedTheme;
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

        //private void MainTab_Loaded(object sender, RoutedEventArgs e)
        //{
        //    MainTab.Items.Add(new TabItem() { Content = new RipplePage(), Header = "Ripple", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new ParticlePage(), Header = "Particle", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new IndicatorPage(), Header = "Indicator", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new ImplicitAnimationPage(), Header = "ImplicitAnimation", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new BigbangPage(), Header = "Bigbang", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new PerspectivePage(), Header = "Perspective", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new CardViewPage(), Header = "CardView", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new DraggedBadgePage(), Header = "DraggedBadge", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new SwipeBackPage(), Header = "SwipeBack", UnloadItemOutsideViewport = false });
        //    MainTab.Items.Add(new TabItem() { Content = new AboutPage(), Header = "About", UnloadItemOutsideViewport = false });
        //}
    }
}
