using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Sample.Models
{
    public class ColorModel : INotifyPropertyChanged
    {
        private Color _Color;
        private string _Name;
        private Brush _Brush;

        public Color Color
        {
            get => _Color;
            set
            {
                _Color = value;
                NotifyPropertyChanged();
            }
        }
        public string Name
        {
            get => _Name;
            set
            {
                _Name = value;
                NotifyPropertyChanged();
            }
        }
        public Brush Brush
        {
            get
            {
                if (_Brush == null) _Brush = new SolidColorBrush(Color);
                return _Brush;
            }
            set
            {
                _Brush = value;
                NotifyPropertyChanged();
            }
        }

        public ColorModel(Color color, string name = null)
        {
            _Color = color;
            if (name == null)
                _Name = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            else _Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
