using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class ListViewModel : INotifyPropertyChanged
    {
        private string _Title;
        private string _Content;
        private Uri _Image;

        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                NotifyPropertyChanged();
            }
        }

        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                NotifyPropertyChanged();
            }
        }

        public Uri Image
        {
            get => _Image;
            set
            {
                _Image = value;
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
