using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender == RipplePageFrame) RipplePageFrame.Navigate(typeof(Views.RipplePage));
            else if (sender == ParticlePageFrame) ParticlePageFrame.Navigate(typeof(Views.ParticlePage));
            else if (sender == TipsRectanglePageFrame) TipsRectanglePageFrame.Navigate(typeof(Views.TipsRectanglePage));
            else if (sender == ImplicitAnimationPageFrame) ImplicitAnimationPageFrame.Navigate(typeof(Views.ImplicitAnimationPage));
            else if (sender == ScrollHeaderPageFrame) ScrollHeaderPageFrame.Navigate(typeof(Views.ScrollHeaderPage));
            else if (sender == AboutPageFrame) AboutPageFrame.Navigate(typeof(Views.AboutPage));
        }
    }
}
