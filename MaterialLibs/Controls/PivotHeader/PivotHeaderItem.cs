using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls.PivotHeader
{
    public class PivotHeaderItem : SelectorItem
    {
        public PivotHeaderItem()
        {
            this.DefaultStyleKey = typeof(PivotHeaderItem);
            this.IsEnabledChanged += (s, a) => UpdateState();
            RegisterPropertyChangedCallback(IsSelectedProperty, IsSelectedPropertyChanged);
        }

        public Rectangle SelectionIndicator { get; private set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SelectionIndicator = GetTemplateChild("SelectionIndicator") as Rectangle;
        }

        private string state = string.Empty;

        public void UpdateState()
        {
            if (IsEnabled)
            {
                if (string.IsNullOrEmpty(state))
                {
                    VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Normal", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, (IsSelected ? "Selected" : string.Empty) + state, true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "Disabled", true);
            }

            if (SelectionIndicator != null)
            {
                SelectionIndicator.Opacity = IsSelected ? 1 : 0;
            }
        }

        private void IsSelectedPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            UpdateState();
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            state = "PointerOver";
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            state = string.Empty;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            state = "Pressed";
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            state = "PointerOver";
        }
    }
}
