using MaterialLibs.Factorys;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Sample.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ChromeFlyoutPage : Page
    {
        public ChromeFlyoutPage()
        {
            this.InitializeComponent();

            var pipe = new ActionEventPipe<RoutedEventArgs>(TestPipe, nameof(TestPipe.Click), () =>
            {
                var result =  (DateTime.Now - LastClick).TotalSeconds < 1;

                LastClick = DateTime.Now;

                return result;
            });

            pipe.EventAttached += (s, a) =>
            {
                System.Diagnostics.Debug.WriteLine($"{count++} pipe!");
            };
        }

        int count;
        DateTime LastClick;

        List<object> list;

        private void MenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
        {
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
        }

        private void add(FrameworkElement ele)
        {
            var list = new List<object>();
            if (this.list != null)
            {
                list.Add(this.list);
            }
            var p = (FrameworkElement)VisualTreeHelper.GetParent(ele);
            if (p == null) return;
            var count = VisualTreeHelper.GetChildrenCount(p);
            for (int i = 0; i < count; i++)
            {
                list.Add((FrameworkElement)VisualTreeHelper.GetChild(p, i));
            }
            this.list = list;
            add(p);
        }
    }
}
