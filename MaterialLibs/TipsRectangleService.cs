using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace MaterialLibs
{
    public class TipsRectangleService : DependencyObject
    {
        private static Dictionary<string, TipsRectangleServiceItem> TokenRectangles = new Dictionary<string, TipsRectangleServiceItem>();
        public static string GetToken(FrameworkElement obj)
        {
            return (string)obj.GetValue(TokenProperty);
        }

        public static void SetToken(FrameworkElement obj, string value)
        {
            obj.SetValue(TokenProperty, value);
        }

        public static readonly DependencyProperty TokenProperty =
            DependencyProperty.RegisterAttached("Token", typeof(string), typeof(TipsRectangleService), new PropertyMetadata(null));

        public static TipsRectangleServiceStates GetState(FrameworkElement obj)
        {
            return (TipsRectangleServiceStates)obj.GetValue(StateProperty);
        }

        public static void SetState(FrameworkElement obj, TipsRectangleServiceStates value)
        {
            obj.SetValue(StateProperty, value);
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached("State", typeof(TipsRectangleServiceStates), typeof(TipsRectangleService), new PropertyMetadata(TipsRectangleServiceStates.None, StatePropertyChanged));

        private static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((int)e.NewValue != (int)e.OldValue && e.NewValue != null)
            {
                var state = (TipsRectangleServiceStates)e.NewValue;
                if (d is FrameworkElement ele)
                {
                    var token = GetToken(ele);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        switch (state)
                        {
                            case TipsRectangleServiceStates.None:
                                {
                                    if (TokenRectangles.ContainsKey(token)) TokenRectangles.Remove(token);
                                }
                                break;
                            case TipsRectangleServiceStates.From:
                                {
                                    if (!TokenRectangles.ContainsKey(token))
                                    {
                                        TokenRectangles[token] = new TipsRectangleServiceItem(ele, TipsRectangleServiceStates.From);
                                    }
                                    else if (TokenRectangles[token].State == TipsRectangleServiceStates.To)
                                    {
                                        TryStartAnimation(token, ele, TokenRectangles[token].Item);
                                        TokenRectangles.Remove(token);
                                    }
                                }
                                break;
                            case TipsRectangleServiceStates.To:
                                {
                                    if (!TokenRectangles.ContainsKey(token))
                                    {
                                        TokenRectangles[token] = new TipsRectangleServiceItem(ele, TipsRectangleServiceStates.To);
                                    }
                                    else if (TokenRectangles[token].State == TipsRectangleServiceStates.From)
                                    {
                                        TryStartAnimation(token, TokenRectangles[token].Item, ele);
                                        TokenRectangles.Remove(token);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void TryStartAnimation(string token, FrameworkElement source, FrameworkElement target)
        {
            if (source.ActualHeight > 0 && source.ActualWidth > 0)
            {
                var service = ConnectedAnimationService.GetForCurrentView();
                if (source != target)
                {
                    service.GetAnimation(token)?.Cancel();
                    service.DefaultDuration = TimeSpan.FromSeconds(0.33d);
                    var animation = service.PrepareToAnimate(token, source);
                    animation.TryStart(target);
                }
            }
        }
    }
    internal class TipsRectangleServiceItem
    {
        public FrameworkElement Item { get; set; }
        public TipsRectangleServiceStates State { get; set; }
        public TipsRectangleServiceItem(FrameworkElement Item, TipsRectangleServiceStates State)
        {
            this.Item = Item;
            this.State = State;
        }
    }
    public enum TipsRectangleServiceStates
    {
        None, From, To
    }

}
