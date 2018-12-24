﻿using Sample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class IndicatorPage : Page
    {
        public IndicatorPage()
        {
            this.InitializeComponent();
            Items = new ObservableCollection<ListViewModel>();
            this.Loaded += TipsRectanglePage_Loaded;

        }

        private async void TipsRectanglePage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Yield();
            for (int i = 0; i < 20; i++)
            {
                Items.Add(new ListViewModel() { Title = "Item" + i, Content = "This is Item" + i, Image = new Uri("ms-appx:///Assets/imgs/" + i + ".jpg") });
                await Task.Yield();
            }
        }

        ObservableCollection<ListViewModel> Items { get; set; }
    }
}
