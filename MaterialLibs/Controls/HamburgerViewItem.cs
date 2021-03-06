﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MaterialLibs.Controls
{
    public class HamburgerViewItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private object _Icon;
        private object _Content;
        private object _Tag;

        public object Icon
        {
            get => _Icon;
            set
            {
                _Icon = value;
                NotifyPropertyChanged();
            }
        }

        public object Content
        {
            get => _Content;
            set
            {
                _Content = value;
                NotifyPropertyChanged();
            }
        }

        public object Tag
        {
            get => _Tag;
            set
            {
                _Tag = value;
                NotifyPropertyChanged();
            }
        }
    }
}
