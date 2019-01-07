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
using Windows.UI.Xaml.Hosting;
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
                    var token = selector.GetHashCode().ToString();
                    //TryStartAnimation(token, newIndicator, oldIndicator);
                    TryStartVisualAnimation(newIndicator, oldIndicator);
                }
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
            OldIndicator.Visibility = Visibility.Collapsed;
            NewIndicator.Visibility = Visibility.Visible;

            //TryStartAnimation(token, NewIndicator, OldIndicator);
            TryStartVisualAnimation(NewIndicator, OldIndicator);

        }

        private static VisualState GetCurrentState(FrameworkElement element, string GroupName)
        {
            if (VisualTreeHelper.GetChildrenCount(element) > 0 && VisualTreeHelper.GetChild(element, 0) is FrameworkElement child)
                return VisualStateManager.GetVisualStateGroups(child).FirstOrDefault(c => c.Name == GroupName)?.CurrentState;
            return null;
        }

        private static void CreateDuration(FrameworkElement element, string groupName, string From, string To)
        {
            if (VisualTreeHelper.GetChildrenCount(element) > 0 && VisualTreeHelper.GetChild(element, 0) is FrameworkElement child)
            {
                var group = VisualStateManager.GetVisualStateGroups(child).FirstOrDefault(c => c.Name == groupName);
                if (group != null)
                {
                    var trans = group.Transitions.FirstOrDefault(c => c.From == From && c.To == To);
                    if (trans != null)
                    {
                        if (trans.GeneratedDuration.HasTimeSpan && trans.GeneratedDuration.TimeSpan < TimeSpan.FromSeconds(0.01)
                            || !trans.GeneratedDuration.HasTimeSpan)
                        {
                            trans.GeneratedDuration = new Duration(TimeSpan.FromSeconds(0.01));
                        }
                    }
                    else
                    {
                        group.Transitions.Add(new VisualTransition()
                        {
                            From = From,
                            To = To,
                            GeneratedDuration = new Duration(TimeSpan.FromSeconds(0.01))
                        });
                    }
                }
            }
        }

        private static void TryStartVisualAnimation(FrameworkElement newIndicator, FrameworkElement oldIndicator)
        {
            var compositor = Window.Current.Compositor;

            var oldSize = new Vector2((float)oldIndicator.ActualWidth, (float)oldIndicator.ActualHeight);
            var newSize = new Vector2((float)newIndicator.ActualWidth, (float)newIndicator.ActualHeight);

            var oldScale = oldSize / newSize;

            var oldOffset = oldIndicator.TransformToVisual(newIndicator).TransformPoint(new Windows.Foundation.Point(0, 0)).ToVector2();

            var old_target = ElementCompositionPreview.GetElementVisual(oldIndicator);
            var new_target = ElementCompositionPreview.GetElementVisual(newIndicator);

            var duration = TimeSpan.FromSeconds(0.23);
            var delay = TimeSpan.FromSeconds(0.01);

            var centerAnimation = compositor.CreateVector3KeyFrameAnimation();
            centerAnimation.InsertExpressionKeyFrame(0f, "Vector3(new_target.Size.X / 2,new_target.Size.Y / 2,0f)");
            centerAnimation.InsertExpressionKeyFrame(1f, "Vector3(new_target.Size.X / 2,new_target.Size.Y / 2,0f)");
            centerAnimation.SetReferenceParameter("new_target", new_target);
            centerAnimation.Duration = duration;

            var offsetAnimation = compositor.CreateVector2KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(0f, "oldOffset");
            offsetAnimation.InsertExpressionKeyFrame(1f, "This.StartingValue");
            offsetAnimation.SetVector2Parameter("oldOffset", oldOffset);
            offsetAnimation.Duration = duration;

            var scaleAnimation = compositor.CreateVector2KeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(0f, "oldScale");
            scaleAnimation.InsertExpressionKeyFrame(1f, "This.StartingValue");
            scaleAnimation.SetVector2Parameter("oldScale", oldScale);
            scaleAnimation.Duration = duration;

            ElementCompositionPreview.SetIsTranslationEnabled(newIndicator, true);

            new_target.StartAnimation("CenterPoint", centerAnimation);
            new_target.StartAnimation("Translation.XY", offsetAnimation);
            new_target.StartAnimation("Scale.XY", scaleAnimation);
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
