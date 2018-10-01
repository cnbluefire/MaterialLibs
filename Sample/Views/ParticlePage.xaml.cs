using Sample.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ParticlePage : Page
    {
        public ParticlePage()
        {
            this.InitializeComponent();
            KnownColors = new ObservableCollection<ColorModel>();
            KnownColors.Add(new ColorModel(Colors.Black,nameof(Colors.Black)));
            KnownColors.Add(new ColorModel(Colors.Red, nameof(Colors.Red)));
            KnownColors.Add(new ColorModel(Colors.Pink, nameof(Colors.Pink)));
            KnownColors.Add(new ColorModel(Colors.LightPink, nameof(Colors.LightPink)));
            KnownColors.Add(new ColorModel(Colors.Green, nameof(Colors.Green)));
            KnownColors.Add(new ColorModel(Colors.LightGreen, nameof(Colors.LightGreen)));
            KnownColors.Add(new ColorModel(Colors.LimeGreen, nameof(Colors.LimeGreen)));
            KnownColors.Add(new ColorModel(Colors.Gray, nameof(Colors.Gray)));
            KnownColors.Add(new ColorModel(Colors.LightGray, nameof(Colors.LightGray)));
            KnownColors.Add(new ColorModel(Colors.DarkGray, nameof(Colors.DarkGray)));
            KnownColors.Add(new ColorModel(Colors.Yellow, nameof(Colors.Yellow)));
            KnownColors.Add(new ColorModel(Colors.LightGoldenrodYellow, nameof(Colors.LightGoldenrodYellow)));
            KnownColors.Add(new ColorModel(Colors.Orange, nameof(Colors.Orange)));
        }

        ObservableCollection<ColorModel> KnownColors { get; set; }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            if(SettingStackPanel.Visibility == Visibility.Visible)
            {
                SettingStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                SettingStackPanel.Visibility = Visibility.Visible;
            }
        }
    }
}
