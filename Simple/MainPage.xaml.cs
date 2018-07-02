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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            ThemeItem = new HamburgerViewItem() { Content = "Theme", Icon = "\uE809", Tag = typeof(RipplePage) };

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
            _HamburgerView.SelectedItem = PrimaryList[0];
            ContentFrame.Navigate(typeof(RipplePage));
        }

        private void _HamburgerView_ItemClick(object sender, MaterialLibs.Controls.HamburgerViewItemClickEventArgs e)
        {

        }

        private void _HamburgerView_SelectionChanging(object sender, MaterialLibs.Controls.HamburgerViewSelectionChangingEventArgs e)
        {

        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            UpdateBackState();
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
    }
}
