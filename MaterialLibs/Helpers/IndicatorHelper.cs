using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MaterialLibs.Helpers
{
    public static class IndicatorHelper
    {
        public static string GetName(DependencyObject obj)
        {
            return (string)obj.GetValue(NameProperty);
        }

        public static void SetName(DependencyObject obj, string value)
        {
            obj.SetValue(NameProperty, value);
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(string), typeof(IndicatorHelper), new PropertyMetadata(null, NamePropertyChanged));


        private static void NamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is string Name)
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    if (d is Selector selector)
                    {
                        selector.SelectionChanged -= OnSelectionChanged;
                        selector.SelectionChanged += OnSelectionChanged;
                    }
                    else if (d is Pivot pivot)
                    {
                        if (IsLoaded(pivot))
                        {
                            TryStartPivotHeaderAnimation(pivot, pivot.SelectedItem, null);
                        }
                        else
                        {
                            pivot.Loaded += _Loaded;
                        }
                        pivot.SelectionChanged -= OnSelectionChanged;
                        pivot.SelectionChanged += OnSelectionChanged;
                    }
                }
                else
                {
                    if (d is Selector selector)
                    {
                        selector.SelectionChanged -= OnSelectionChanged;
                    }
                    else if (d is Pivot pivot)
                    {
                        pivot.SelectionChanged -= OnSelectionChanged;
                    }
                }
            }
        }

        private static void _Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Pivot pivot)
            {
                pivot.Loaded -= _Loaded;
                TryStartPivotHeaderAnimation(pivot, pivot.SelectedItem, null);
            }
            if (sender is Selector selector)
            {
                selector.Loaded -= _Loaded;
                TryStartSelectorAnimation(selector, selector.SelectedItem, null);

            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1 || e.RemovedItems.Count != 1) return;
            if (sender is Selector selector)
            {
                if (IsLoaded(selector))
                    TryStartSelectorAnimation(selector, e.AddedItems.FirstOrDefault(), e.RemovedItems.FirstOrDefault());
            }
            if (sender is Pivot pivot)
            {
                TryStartPivotHeaderAnimation(pivot, e.AddedItems.FirstOrDefault(), e.RemovedItems.FirstOrDefault());
            }
        }

        private static async void TryStartSelectorAnimation(Selector selector, object NewItem, object OldItem)
        {
            if (NewItem == null || OldItem == null) return;
            if (selector is Windows.UI.Xaml.Controls.ListViewBase listView)
            {
                if (listView.SelectionMode == ListViewSelectionMode.None || listView.SelectionMode == ListViewSelectionMode.Multiple) return;
            }
            else if (selector is ListBox listBox)
            {
                if (listBox.SelectionMode == SelectionMode.Multiple) return;
            }

            var name = GetName(selector);

            if (string.IsNullOrEmpty(name)) return;

            var oldContainer = selector.ContainerFromItem(OldItem) as FrameworkElement;
            var newContainer = selector.ContainerFromItem(NewItem) as FrameworkElement;

            var oldIndicator = oldContainer?.VisualTreeFindName(name);
            var newIndicator = newContainer?.VisualTreeFindName(name);

            if (newIndicator != null)
            {

                if (oldIndicator != null)
                {
                    var oldContainerState = GetCurrentState(oldContainer, "CommonStates");
                    var newContainerState = GetCurrentState(newContainer, "CommonStates");

                    var oldVisibility = oldIndicator.Visibility;
                    var oldOpacity = oldIndicator.Opacity;

                    bool HasChanged = oldVisibility != newIndicator.Visibility || oldOpacity != newIndicator.Opacity;

                    if (HasChanged)
                    {
                        oldIndicator.Visibility = newIndicator.Visibility;
                        oldIndicator.Opacity = newIndicator.Opacity;
                    }

                    var token = selector.GetHashCode().ToString();
                    TryStartAnimation(token, newIndicator, oldIndicator);

                    if (HasChanged)
                    {
                        //为了动画连贯性，新旧指示器至少有很小一段时间同时存在，但是这段时间里控件的VisualState可能会发生变化
                        //所以：
                        //1.先获取当前也就是最终State
                        //2.进行延时
                        //3.将Container还原到延时之前的State
                        //4.将Visibility和Opacity设置回最终状态的值
                        //5.将Container设置为最终State
                        var lastOldState = GetCurrentState(oldContainer, "CommonStates");
                        if (oldContainer is Control && oldContainerState != null && lastOldState != null)
                        {
                            await Task.Delay(50);
                            VisualStateManager.GoToState((Control)oldContainer, oldContainerState.Name, false);
                            oldIndicator.Visibility = oldVisibility;
                            oldIndicator.Opacity = oldOpacity;
                            VisualStateManager.GoToState((Control)oldContainer, lastOldState.Name, false);
                        }
                    }
                }
            }
            else
            {
                VisualStateManager.GoToState((Control)newContainer, "Selected", false);
            }
        }


        private static void TryStartPivotHeaderAnimation(Pivot pivot, object NewItem, object OldItem)
        {
            if (NewItem == null) return;

            var NewIndex = -1;
            var OldIndex = -1;

            if (pivot.ContainerFromItem(OldItem) is PivotItem oldContainer)
            {
                OldIndex = pivot.IndexFromContainer(oldContainer);
            }

            if (pivot.ContainerFromItem(NewItem) is PivotItem newContainer)
            {
                NewIndex = pivot.IndexFromContainer(newContainer);
            }

            if (NewIndex >= 0)
            {
                var name = GetName(pivot);

                var Headers = pivot.VisualTreeFindAll<PivotHeaderPanel>();
                var Header = Headers.FirstOrDefault(c => c.Name == "Header");
                var StaticHeader = Headers.FirstOrDefault(c => c.Name == "StaticHeader");

                var newIndicator = GetPivotHeaderIndicator(Header, name, NewIndex);
                var newStaticIndicator = GetPivotHeaderIndicator(StaticHeader, name, NewIndex);

                if (OldIndex >= 0)
                {
                    var oldIndicator = GetPivotHeaderIndicator(Header, name, OldIndex);
                    var oldStaticIndicator = GetPivotHeaderIndicator(StaticHeader, name, OldIndex);

                    if (Header != null && newIndicator != null && oldIndicator != null)
                    {
                        TryStartPivotHeaderAnimation(Header, newIndicator, oldIndicator);
                    }
                    if (StaticHeader != null && newStaticIndicator != null && oldStaticIndicator != null)
                    {
                        TryStartPivotHeaderAnimation(StaticHeader, newStaticIndicator, oldStaticIndicator);
                    }
                }
                else
                {
                    if (newIndicator != null)
                    {
                        newIndicator.Visibility = Visibility.Visible;
                    }
                    if (newStaticIndicator != null)
                    {
                        newStaticIndicator.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private static FrameworkElement GetPivotHeaderIndicator(PivotHeaderPanel panel, string Name, int Index)
        {
            if (panel == null) return null;
            var container = panel.Children[Index] as FrameworkElement;
            return container?.VisualTreeFindName(Name);
        }

        private static void TryStartPivotHeaderAnimation(PivotHeaderPanel panel, FrameworkElement NewIndicator, FrameworkElement OldIndicator)
        {
            OldIndicator.Visibility = Visibility.Visible;
            NewIndicator.Visibility = Visibility.Visible;

            var token = panel.GetHashCode().ToString();
            TryStartAnimation(token, NewIndicator, OldIndicator);

            OldIndicator.Visibility = Visibility.Collapsed;
            NewIndicator.Visibility = Visibility.Visible;
        }

        private static VisualState GetCurrentState(FrameworkElement element, string GroupName)
        {
            if (VisualTreeHelper.GetChildrenCount(element) > 0 && VisualTreeHelper.GetChild(element, 0) is FrameworkElement child)
                return VisualStateManager.GetVisualStateGroups(child).FirstOrDefault(c => c.Name == GroupName)?.CurrentState;
            return null;
        }

        private static void TryStartAnimation(string token, FrameworkElement newIndicator, FrameworkElement oldIndicator)
        {
            try
            {
                if (newIndicator != oldIndicator)
                {
                    if (IsLoaded(oldIndicator))
                    {
                        var service = ConnectedAnimationService.GetForCurrentView();
                        service.GetAnimation(token)?.Cancel();
                        var animation = service.PrepareToAnimate(token, oldIndicator);
                        animation.TryStart(newIndicator);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private static bool IsLoaded(FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element);
            return element.ActualHeight > 0 || element.ActualWidth > 0;
        }
    }
}
