using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MaterialLibs.Controls.PivotHeader
{
    public class PivotHeader : ItemsControl
    {
        //private object lastItem;

        public PivotHeader()
        {
            this.DefaultStyleKey = typeof(PivotHeader);
        }

        #region Const Values

        private static readonly Vector2 c_frame1point1 = new Vector2(0.9f, 0.1f);
        private static readonly Vector2 c_frame1point2 = new Vector2(0.7f, 0.4f);
        private static readonly Vector2 c_frame2point1 = new Vector2(0.1f, 0.9f);
        private static readonly Vector2 c_frame2point2 = new Vector2(0.2f, 1f);

        #endregion Const Values

        #region Composition Resource

        Compositor compositor => Window.Current.Compositor;

        CompositionPropertySet pivotScrollPropset;

        #endregion Composition Resource


        #region ItemsControl Overrides

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotHeaderItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotHeaderItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is PivotHeaderItem container)
            {
                container.PointerExited += Container_PointerExited;
                container.PointerEntered += Container_PointerEntered;
                container.PointerPressed += Container_PointerPressed;
                container.PointerReleased += Container_PointerReleased;
                container.PointerCaptureLost += Container_PointerCaptureLost;
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (element is PivotHeaderItem container)
            {
                container.PointerExited -= Container_PointerExited;
                container.PointerEntered -= Container_PointerEntered;
                container.PointerPressed -= Container_PointerPressed;
                container.PointerReleased -= Container_PointerReleased;
                container.PointerCaptureLost -= Container_PointerCaptureLost;
            }
        }

        #endregion ItemsControl Overrides

        #region Container Event Methods

        private void Container_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((PivotHeaderItem)sender).UpdateState();
        }

        private void Container_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((PivotHeaderItem)sender).UpdateState();
        }

        private void Container_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ((PivotHeaderItem)sender).UpdateState();
        }

        private void Container_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var container = (PivotHeaderItem)sender;
            container.IsSelected = true;
            container.UpdateState();
            SelectedIndex = IndexFromContainer(container);
        }

        private void Container_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            ((PivotHeaderItem)sender).UpdateState();
        }

        #endregion Container Event Methods

        #region Dependency Properties

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(PivotHeader), new PropertyMetadata(-1, (s, a) =>
            {
                if (a.OldValue != a.NewValue)
                {
                    if (s is PivotHeader sender)
                    {
                        var newIndex = (int)a.NewValue;
                        var oldIndex = (int)a.OldValue;

                        if (newIndex < -1)
                        {
                            throw new IndexOutOfRangeException();
                        }

                        object newItem = null;

                        if (oldIndex > -1 && sender.ContainerFromIndex((int)a.OldValue) is PivotHeaderItem oldContainer)
                        {
                            oldContainer.IsSelected = false;
                        }

                        //get new item
                        if (newIndex > -1)
                        {
                            if (sender.ContainerFromIndex(newIndex) is PivotHeaderItem newContainer)
                            {
                                newContainer.IsSelected = true;

                                newItem = sender.ItemFromContainer(newContainer);

                                var oldItem = sender.SelectedItem;
                                sender.SelectedItem = newItem;
                                sender.TryStartAnimationWithIndex(oldIndex, newIndex);
                                sender.OnSelectionChanged(oldItem, newItem);
                            }
                            else
                            {
                                throw new ArgumentException("Value does not fall within the expected range.");
                            }
                        }
                        else if (newIndex == -1)
                        {
                            var oldItem = sender.SelectedItem;
                            sender.SelectedItem = null;
                            sender.OnSelectionChanged(oldItem, sender.SelectedItem);
                        }
                        else
                        {
                            throw new ArgumentException("Value does not fall within the expected range.");
                        }
                    }
                }
            }));



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(PivotHeader), new PropertyMetadata(null, (s, a) =>
            {
                if (a.OldValue != a.NewValue)
                {
                    if (s is PivotHeader sender)
                    {
                        if (a.NewValue == null)
                        {
                            sender.SelectedIndex = -1;
                        }
                        else
                        {
                            var container = sender.ContainerFromItem(a.NewValue);
                            if (container == null)
                            {
                                sender.SelectedItem = null;
                            }
                            else
                            {
                                sender.SelectedIndex = sender.IndexFromContainer(container);
                            }
                        }
                    }
                }
            }));



        public Pivot Target
        {
            get { return (Pivot)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Pivot), typeof(PivotHeader), new PropertyMetadata(null, (s, a) =>
            {

            }));




        #endregion Dependency Properties

        #region Events

        protected virtual void OnSelectionChanged(object oldItem, object newItem)
        {
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(oldItem, newItem));
        }

        public event SelectionChangedEventHandler SelectionChanged;

        #endregion Events

        #region Compositions

        private void InitComposition()
        {

        }

        private void TryStartAnimationWithIndex(int oldIndex, int newIndex)
        {
            var oldContainer = ContainerFromIndex(oldIndex) as PivotHeaderItem;
            var newContainer = ContainerFromIndex(newIndex) as PivotHeaderItem;

            if (oldContainer == null)
            {
                if (newContainer != null)
                {
                    newContainer.IsSelected = true;
                }
                return;
            }
            else
            {
                oldContainer.IsSelected = false;
            }

            if (newContainer != null)
            {
                TryStartAnimationWithContainer(oldContainer, newContainer);
                newContainer.IsSelected = true;
            }

        }

        private static void ResetCompositionValue(FrameworkElement element)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.StopAnimation("Translation.XY");
            visual.StopAnimation("Scale");
            visual.StopAnimation("CenterPoint");

            visual.Properties.InsertVector3("Translation", Vector3.Zero);
            visual.Scale = Vector3.One;
            visual.CenterPoint = Vector3.Zero;
        }

        private void TryStartAnimationWithContainer(PivotHeaderItem oldContainer, PivotHeaderItem newContainer)
        {
            ResetCompositionValue(oldContainer);

            var oldSize = new Vector2((float)oldContainer.SelectionIndicator.ActualWidth, (float)oldContainer.SelectionIndicator.ActualHeight);
            var newSize = new Vector2((float)newContainer.SelectionIndicator.ActualWidth, (float)newContainer.SelectionIndicator.ActualHeight);

            var oldScale = oldSize / newSize;

            var oldOffset = newContainer.SelectionIndicator.TransformToVisual(oldContainer.SelectionIndicator).TransformPoint(new Windows.Foundation.Point(0, 0)).ToVector2();

            float startx = 0, endx = 0, starty = 0, endy = 0;

            if (oldOffset.X > 0)
            {
                endx = newSize.X;
            }
            else
            {
                startx = newSize.X;
                oldOffset.X = oldOffset.X + newSize.X - oldSize.X;
            }

            if (oldOffset.Y > 0)
            {
                endy = newSize.Y;
            }
            else
            {
                starty = newSize.Y;
                oldOffset.Y = oldOffset.Y + newSize.Y - oldSize.Y;
            }

            var old_target = ElementCompositionPreview.GetElementVisual(oldContainer.SelectionIndicator);
            var new_target = ElementCompositionPreview.GetElementVisual(newContainer.SelectionIndicator);

            var duration = TimeSpan.FromSeconds(0.6d);

            var standard = compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));

            var singleStep = compositor.CreateStepEasingFunction();
            singleStep.IsFinalStepSingleFrame = true;

            var centerAnimation = compositor.CreateVector3KeyFrameAnimation();
            centerAnimation.InsertExpressionKeyFrame(0f, "Vector3(startx,starty,0f)", singleStep);
            centerAnimation.InsertExpressionKeyFrame(0.333f, "Vector3(endx,endy,0f)", singleStep);
            centerAnimation.SetScalarParameter("startx", startx);
            centerAnimation.SetScalarParameter("starty", starty);
            centerAnimation.SetScalarParameter("endx", endx);
            centerAnimation.SetScalarParameter("endy", endy);
            centerAnimation.Duration = duration;

            var offsetAnimation = compositor.CreateVector2KeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(0f, "-oldOffset", singleStep);
            offsetAnimation.InsertExpressionKeyFrame(0.333f, "This.StartingValue", singleStep);
            offsetAnimation.SetVector2Parameter("oldOffset", oldOffset);
            offsetAnimation.Duration = duration;

            var scaleAnimation = compositor.CreateVector2KeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(0f, "oldScale", standard);
            scaleAnimation.InsertExpressionKeyFrame(0.333f, "(target.Size + abs(oldOffset)) / target.Size",
                compositor.CreateCubicBezierEasingFunction(c_frame1point1, c_frame1point2));
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.StartingValue",
                compositor.CreateCubicBezierEasingFunction(c_frame2point1, c_frame2point2));
            scaleAnimation.SetVector2Parameter("oldScale", oldScale);
            scaleAnimation.SetVector2Parameter("oldOffset", oldOffset);
            scaleAnimation.SetReferenceParameter("target", new_target);
            scaleAnimation.SetReferenceParameter("old", old_target);
            scaleAnimation.Duration = duration;

            ElementCompositionPreview.SetIsTranslationEnabled(newContainer.SelectionIndicator, true);

            new_target.StartAnimation("CenterPoint", centerAnimation);
            new_target.StartAnimation("Translation.XY", offsetAnimation);
            new_target.StartAnimation("Scale.XY", scaleAnimation);
        }
        #endregion Compositions
    }

    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);


    public class SelectionChangedEventArgs
    {
        public SelectionChangedEventArgs(object oldItem, object newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }


        public object OldItem { get; }

        public object NewItem { get; }
    }
}
