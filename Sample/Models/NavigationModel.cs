using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class NavigationModel : INotifyPropertyChanged
    {
        private string _Title;
        private Type _PageType;

        public Type PageType
        {
            get => _PageType;
            set
            {
                _PageType = value;
                NotifyPropertyChanged();
            }
        }
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
