using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public sealed partial class DraggedBadgePage : Page
    {
        public DraggedBadgePage()
        {
            this.InitializeComponent();
            var rnd = new Random();
            list = new ObservableCollection<TestClass>();
            list.Add(new TestClass("1", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("2", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("3", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("4", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("5", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("6", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("7", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("8", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("9", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("10", rnd.Next(1, 100).ToString()));
            list.Add(new TestClass("11", rnd.Next(1, 100).ToString()));
        }

        ObservableCollection<TestClass> list { get; set; }

        private void _DragCompleted(object sender, MaterialLibs.Controls.DragCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(((MaterialLibs.Controls.DraggedBadge)sender).Content);
        }
    }



    public class TestClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _Content;
        private string _Count;

        public TestClass(string content, string count)
        {
            Content = content;
            Count = count;
        }

        public string Content { get => _Content; set { _Content = value; OnPropertyChanged(); } }
        public string Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }
    }

}
