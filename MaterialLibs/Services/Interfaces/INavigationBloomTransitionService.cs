using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MaterialLibs.Services.Interfaces
{
    public interface INavigationBloomTransitionService
    {
        Frame CurrentFrame { get; set; }

        Task<bool> NavigateAndBloomFromUIElementAsync(UIElement element, Color color, Type sourcePageType);
        Task<bool> NavigateAndBloomFromUIElementAsync(UIElement element, Color color, Type sourcePageType, object parameter);

        Task<bool> NavigateAndBloomFromPositionAsync(Point Point, Color color, Type sourcePageType);
        Task<bool> NavigateAndBloomFromPositionAsync(Point Point, Color color, Type sourcePageType, object parameter);

    }
}
