using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Simple.Converter
{
    public class RippleHelperStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool v)
            {
                if (v) return MaterialLibs.RippleHelperState.Pressed;
                else return MaterialLibs.RippleHelperState.Released;
            }
            return MaterialLibs.RippleHelperState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TrueFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b) return !b;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b) return !b;
            return false;
        }
    }
}
