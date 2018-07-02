using Simple.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UltraBook.Controls;
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

namespace Simple
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
            };

            SecondaryList = new ObservableCollection<HamburgerViewItem>()
            {
                ThemeItem,
                new HamburgerViewItem(){Content = "About",Icon = "\uED54",Tag = typeof(AboutPage)},
            };
        }

        HamburgerViewItem ThemeItem { get; set; }
        public ObservableCollection<HamburgerViewItem> PrimaryList { get; set; }
        public ObservableCollection<HamburgerViewItem> SecondaryList { get; set; }

        private void Page_Loaded(object sender, RoutedEventArgs e)
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
            ContentFrame.Navigate(typeof(RipplePage), new SuppressNavigationTransitionInfo());
        }

        private void _HamburgerView_ItemClick(object sender, MaterialLibs.Controls.HamburgerViewItemClickEventArgs e)
        {
            if(e.ClickedItem == ThemeItem)
            {
                UpdateTheme(RequestedTheme == ElementTheme.Dark);
            }
            else
            {
                ContentFrame.Navigate((Type)((HamburgerViewItem)e.ClickedItem).Tag,null, new SuppressNavigationTransitionInfo());
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
            if (Light) RequestedTheme = ElementTheme.Light;
            else RequestedTheme = ElementTheme.Dark;
        }

        private void UpdateBackState()
        {
            if (ContentFrame.CanGoBack)
            {
                _HamburgerView.IsBackButtonEnable = true;
            }
            else
            {
                _HamburgerView.IsBackButtonEnable = false;
            }
        }

        private void _HamburgerView_BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack(new SuppressNavigationTransitionInfo());
            }
        }
    }
}
