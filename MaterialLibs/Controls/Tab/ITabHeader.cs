using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;

namespace MaterialLibs.Controls.Tab
{
    public interface ITabHeader
    {
        /// <summary>
        /// 设置关联的Tab宽度
        /// </summary>
        /// <param name="Width"></param>
        void SetTabsWidth(double Width);

        /// <summary>
        /// 设置关联的Tab滚动条信息
        /// </summary>
        /// <param name="ScrollPropertySet"></param>
        void SetTabsRootScrollPropertySet(CompositionPropertySet ScrollPropertySet);

        /// <summary>
        /// 当关联的滚动条加载完成时
        /// </summary>
        /// <returns></returns>
        Task OnTabsLoadedAsync();

        /// <summary>
        /// Indicator与关联的滚动条同步
        /// </summary>
        /// <param name="Index"></param>
        void SyncSelection(int Index);

        event SelectionChangedEventHandler SelectionChanged;
        ItemCollection Items { get; }
        int SelectedIndex { get; set; }
    }
}
