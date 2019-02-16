using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public class PivotHeader : ListView
    {
        public PivotHeader()
        {
            this.DefaultStyleKey = typeof(PivotHeader);
            RegisterPropertyChangedCallback(SelectedIndexProperty, SelectedIndexPropertyChanged);
            Loaded += PivotHeader_Loaded;
        }

        #region Const Values

        private static readonly Vector2 c_frame1point1 = new Vector2(0.9f, 0.1f);
        private static readonly Vector2 c_frame1point2 = new Vector2(0.7f, 0.4f);
        private static readonly Vector2 c_frame2point1 = new Vector2(0.1f, 0.9f);
        private static readonly Vector2 c_frame2point2 = new Vector2(0.2f, 1f);

        #endregion Const Values

        #region Composition Resource

        Compositor compositor => Window.Current.Compositor;

        #endregion Composition Resource

        #region Fields

        private int oldSelectedIndex = -1;

        #endregion Fields

        #region Compositions

        private void TryStartAnimationWithIndex(int oldIndex, int newIndex)
        {

            if (oldIndex == -1)
            {
                var newContainer = ContainerFromIndex(newIndex) as PivotHeaderItem;
                if (newContainer != null)
                {
                    newContainer.IsSelected = true;
                }
                return;
            }
            else
            {
                var oldContainer = ContainerFromIndex(oldIndex) as PivotHeaderItem;
                var newContainer = ContainerFromIndex(newIndex) as PivotHeaderItem;

                oldContainer.IsSelected = false;

                if (newContainer != null)
                {
                    TryStartAnimationWithContainer(oldContainer, newContainer);
                    newContainer.IsSelected = true;
                    newContainer.StartBringIntoView();
                }
            }
        }

        private static void ResetCompositionValue(FrameworkElement element)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.StopAnimation("Translation.X");
            visual.StopAnimation("Scale.X");
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

            var oldScale = oldSize.X / newSize.X;

            var oldOffset = newContainer.SelectionIndicator.TransformToVisual(oldContainer.SelectionIndicator).TransformPoint(new Windows.Foundation.Point(0, 0)).ToVector2();

            float startx = 0, endx = 0;
            var oldOffsetX = oldOffset.X;

            if (oldOffset.X > 0)
            {
                endx = newSize.X;
            }
            else
            {
                startx = newSize.X;
                oldOffsetX = oldOffset.X + newSize.X - oldSize.X;
            }


            var old_target = ElementCompositionPreview.GetElementVisual(oldContainer.SelectionIndicator);
            var new_target = ElementCompositionPreview.GetElementVisual(newContainer.SelectionIndicator);

            var duration = TimeSpan.FromSeconds(0.6d);

            var standard = compositor.CreateCubicBezierEasingFunction(new Vector2(0.8f, 0.0f), new Vector2(0.2f, 1.0f));

            var singleStep = compositor.CreateStepEasingFunction();
            singleStep.IsFinalStepSingleFrame = true;

            var centerAnimation = compositor.CreateVector3KeyFrameAnimation();
            centerAnimation.InsertExpressionKeyFrame(0f, "Vector3(startx,0f,0f)", singleStep);
            centerAnimation.InsertExpressionKeyFrame(0.333f, "Vector3(endx,0f,0f)", singleStep);
            centerAnimation.SetScalarParameter("startx", startx);
            centerAnimation.SetScalarParameter("endx", endx);
            centerAnimation.Duration = duration;

            var offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertExpressionKeyFrame(0f, "-oldOffsetX", singleStep);
            offsetAnimation.InsertExpressionKeyFrame(0.333f, "This.StartingValue", singleStep);
            offsetAnimation.SetScalarParameter("oldOffsetX", oldOffsetX);
            offsetAnimation.Duration = duration;

            var scaleAnimation = compositor.CreateScalarKeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(0f, "oldScale", standard);
            scaleAnimation.InsertExpressionKeyFrame(0.333f, "(target.Size.X + abs(oldOffsetX)) / target.Size.X",
                compositor.CreateCubicBezierEasingFunction(c_frame1point1, c_frame1point2));
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.StartingValue",
                compositor.CreateCubicBezierEasingFunction(c_frame2point1, c_frame2point2));
            scaleAnimation.SetScalarParameter("oldScale", oldScale);
            scaleAnimation.SetScalarParameter("oldOffsetX", oldOffsetX);
            scaleAnimation.SetReferenceParameter("target", new_target);
            scaleAnimation.SetReferenceParameter("old", old_target);
            scaleAnimation.Duration = duration;

            ElementCompositionPreview.SetIsTranslationEnabled(newContainer.SelectionIndicator, true);

            new_target.StartAnimation("CenterPoint", centerAnimation);
            new_target.StartAnimation("Translation.X", offsetAnimation);
            new_target.StartAnimation("Scale.X", scaleAnimation);
        }
        #endregion Compositions

        #region ItemsControl Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotHeaderItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotHeaderItem();
        }

        #endregion ItemsControl Overrides

        #region Event Methods

        private void PivotHeader_Loaded(object sender, RoutedEventArgs e)
        {
            if (SelectedIndex > -1)
            {
                if (ContainerFromIndex(SelectedIndex) is PivotHeaderItem container)
                {
                    container.IsSelected = true;
                }
            }
        }

        #endregion Event Methods

        #region Property Changed Events

        private void SelectedIndexPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            var index = (int)sender.GetValue(dp);
            TryStartAnimationWithIndex(oldSelectedIndex, index);
            oldSelectedIndex = index;
        }

        #endregion Property Changed Events
    }
}
