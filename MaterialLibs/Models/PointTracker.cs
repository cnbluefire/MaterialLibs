using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace MaterialLibs.Models
{
    public class PointTracker : Control
    {
        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(PointTracker), new PropertyMetadata(new Point(0, 0), (s, a) =>
            {
                if(a.NewValue != a.OldValue)
                {
                    if(s is PointTracker sender)
                    {
                        sender.OnPositionChanged((Point)a.NewValue);
                    }
                }
            }));

        public event PositionChangedEventHandler PositionChanged;
        private void OnPositionChanged(Point position)
        {
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(position));
        }
    }

    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs args);

    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(Point position)
        {
            Position = position;
        }

        public Point Position { get; private set; }
    }
}
