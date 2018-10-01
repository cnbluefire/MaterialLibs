using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MaterialLibs.Controls
{
    public class DivideView : Panel
    {
        public DivideView()
        {

        }

        double _MaxWidth;
        double _MaxHeight;

        protected override Size MeasureOverride(Size availableSize)
        {
            _MaxWidth = 0;
            _MaxHeight = 0;
            foreach (var item in Children)
            {
                item.Measure(availableSize);
                _MaxWidth = Math.Max(_MaxWidth, item.DesiredSize.Width);
                _MaxHeight = Math.Max(_MaxHeight, item.DesiredSize.Height);
            }
            if (_MaxWidth * Children.Count > availableSize.Width)
            {
                _MaxWidth = availableSize.Width / Children.Count;
            }
            if(_MaxHeight > availableSize.Height)
            {
                _MaxHeight = availableSize.Height;
            }
            foreach (var item in Children)
            {
                item.InvalidateMeasure();
                item.Measure(new Size(_MaxWidth,_MaxHeight));
            }
            return new Size(Math.Min(availableSize.Width, _MaxWidth * Children.Count), _MaxHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(i * _MaxWidth, 0, _MaxWidth, _MaxHeight));
            }
            return new Size(_MaxWidth * Children.Count, _MaxHeight);
        }
    }
}
