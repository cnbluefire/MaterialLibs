using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MaterialLibs.Animations
{
    public class StartingKeyFrame : DependencyObject, IAnimationKeyFrameBase
    {
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(StartingKeyFrame), new PropertyMetadata(0d, ProgressPropertyChanged));


        public string Value
        {
            get => "this.StartingValue";
            set { }
        }

        private static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                if (d is StartingKeyFrame sender)
                {
                    sender.OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
        }
    }
}
