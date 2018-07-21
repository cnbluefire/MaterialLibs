﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Simple.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CardViewPage : Page
    {
        public CardViewPage()
        {
            this.InitializeComponent();
            items = new ObservableCollection<string>();
            for (int i = 0; i < 30; i++)
            {
                items.Add(i.ToString());
            }
        }

        ObservableCollection<string> items { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Card.IsOpen = true;
        }
    }
}