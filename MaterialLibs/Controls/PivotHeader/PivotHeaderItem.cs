using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MaterialLibs.Controls.PivotHeader
{
    public class PivotHeaderItem : ListViewItem
    {
        public PivotHeaderItem()
        {
            this.DefaultStyleKey = typeof(PivotHeaderItem);
            this.IsEnabledChanged += (s, a) => UpdateState();
            RegisterPropertyChangedCallback(IsSelectedProperty, IsSelectedPropertyChanged);
        }

        #region Fields

        public Rectangle SelectionIndicator { get; private set; }
        private string state = string.Empty;

        #endregion Fields

        #region Property Changed Events

        private void IsSelectedPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            UpdateState();
        }

        #endregion Property Changed Events

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SelectionIndicator = GetTemplateChild("SelectionIndicator") as Rectangle;
            UpdateOpacity();
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

        #endregion Overrides

        #region Update States

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
            UpdateOpacity();
        }

        private void UpdateOpacity()
        {
            if (SelectionIndicator != null)
            {
                SelectionIndicator.Opacity = IsSelected ? 1 : 0;
            }
        }

        #endregion Update States

    }
}
