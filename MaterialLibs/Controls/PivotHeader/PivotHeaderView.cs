using MaterialLibs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace MaterialLibs.Controls.PivotHeader
{
    [ContentProperty(Name = "Pivot")]
    public class PivotHeaderView : Control
    {
        public PivotHeaderView()
        {
            this.DefaultStyleKey = typeof(PivotHeaderView);
        }

        private PivotHeader PivotHeader;

        #region Overrides

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PivotHeader = GetTemplateChild("PivotHeader") as PivotHeader;
            if (Pivot != null)
            {
                UpdatePivot(Pivot);
            }
        }

        #endregion Overrides

        #region Update Pivot

        private void UpdatePivot(Pivot oldPivot, Pivot newPivot)
        {
            if (oldPivot != null)
            {
                SetPivotHeaderVisibility(oldPivot, true);
                oldPivot.Items.VectorChanged -= Items_VectorChanged;
            }

            if (newPivot != null)
            {
                if (PivotHeader != null)
                {
                    UpdatePivot(newPivot);
                }
            }
        }

        private void UpdatePivot(Pivot pivot)
        {
            if (pivot.ItemsSource == null)
            {
                pivot.Items.VectorChanged += Items_VectorChanged;
                ResetPivotHeaderItems();
            }
            else
            {
                SetItemSourceBinding(pivot);
            }

            SetTemplateSourceBinding(pivot);

            if (IsLoaded(pivot))
            {
                SetPivotHeaderVisibility(pivot, false);
                SetIndexBinding(pivot);
            }
            else
            {
                pivot.Loaded += Pivot_Loaded;
            }
        }

        #region Set Bindings

        private void SetIndexBinding(Pivot pivot)
        {
            if (Pivot != null)
            {
                var indexBinding = new Binding();
                indexBinding.Source = pivot;
                indexBinding.Path = new PropertyPath("SelectedIndex");
                indexBinding.Mode = BindingMode.TwoWay;
                PivotHeader.SetBinding(PivotHeader.SelectedIndexProperty, indexBinding);
            }
        }

        private void SetItemSourceBinding(Pivot pivot)
        {
            if (Pivot != null)
            {
                var sourceBinding = new Binding();
                sourceBinding.Source = pivot;
                sourceBinding.Mode = BindingMode.OneWay;
                sourceBinding.Path = new PropertyPath("ItemsSource");
                PivotHeader.SetBinding(PivotHeader.ItemsSourceProperty, sourceBinding);
            }
        }

        private void SetTemplateSourceBinding(Pivot pivot)
        {
            if (Pivot != null)
            {
                var templateBinding = new Binding();
                templateBinding.Source = pivot;
                templateBinding.Path = new PropertyPath("HeaderTemplate");
                templateBinding.Mode = BindingMode.OneWay;
                PivotHeader.SetBinding(PivotHeader.ItemTemplateProperty, templateBinding);
            }
        }

        #endregion Set Bindings

        #region Items Operations

        private void Items_VectorChanged(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            switch (@event.CollectionChange)
            {
                case Windows.Foundation.Collections.CollectionChange.ItemInserted:
                    InsertHeaderItemAt((int)@event.Index);
                    break;
                case Windows.Foundation.Collections.CollectionChange.ItemChanged:
                    UpdateHeaderItemAt((int)@event.Index);
                    break;
                case Windows.Foundation.Collections.CollectionChange.ItemRemoved:
                    RemoveHeaderItemAt((int)@event.Index);
                    break;
                case Windows.Foundation.Collections.CollectionChange.Reset:
                    ResetPivotHeaderItems();
                    break;
            }
        }

        private void InsertHeaderItemAt(int index)
        {
            if (Pivot != null && PivotHeader != null)
            {
                var item = Pivot.Items[index];

                if (item is PivotItem pItem)
                {
                    PivotHeader.Items.Insert(index, new PivotHeaderItem() { Content = pItem.Header });
                }
                else
                {
                    PivotHeader.Items.Insert(index, item);
                }
            }
        }

        private void RemoveHeaderItemAt(int index)
        {
            if (Pivot != null && PivotHeader != null)
            {
                PivotHeader.Items.RemoveAt(index);
            }
        }

        private void UpdateHeaderItemAt(int index)
        {
            if (Pivot != null && PivotHeader != null)
            {
                PivotHeader.Items.RemoveAt(index);
                PivotHeader.Items.Add(Pivot.Items[index]);
            }
        }

        private void ResetPivotHeaderItems()
        {
            if (Pivot != null && PivotHeader != null)
            {
                PivotHeader.Items.Clear();
                foreach (var item in Pivot.Items)
                {
                    PivotHeader.Items.Add(item);
                }
            }
        }

        #endregion Items Operations

        private void Pivot_Loaded(object sender, RoutedEventArgs e)
        {
            var pivot = (Pivot)sender;
            pivot.Loaded -= Pivot_Loaded;
            SetIndexBinding(pivot);
            SetPivotHeaderVisibility(pivot, false);

        }

        #endregion Update Pivot

        #region Dependency Properties

        public Pivot Pivot
        {
            get { return (Pivot)GetValue(PivotProperty); }
            set { SetValue(PivotProperty, value); }
        }

        public static readonly DependencyProperty PivotProperty =
            DependencyProperty.Register("Pivot", typeof(Pivot), typeof(PivotHeaderView), new PropertyMetadata(null, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is PivotHeaderView sender)
                    {
                        sender.UpdatePivot(a.OldValue as Pivot, a.NewValue as Pivot);
                    }
                }
            }));




        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(PivotHeaderView), new PropertyMetadata(null));


        public object LeftHeader
        {
            get { return (object)GetValue(LeftHeaderProperty); }
            set { SetValue(LeftHeaderProperty, value); }
        }

        public static readonly DependencyProperty LeftHeaderProperty =
            DependencyProperty.Register("LeftHeader", typeof(object), typeof(PivotHeaderView), new PropertyMetadata(null));



        public object RightHeader
        {
            get { return (object)GetValue(RightHeaderProperty); }
            set { SetValue(RightHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightHeaderProperty =
            DependencyProperty.Register("RightHeader", typeof(object), typeof(PivotHeaderView), new PropertyMetadata(null));



        public DataTemplate LeftHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LeftHeaderTemplateProperty); }
            set { SetValue(LeftHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftHeaderTemplateProperty =
            DependencyProperty.Register("LeftHeaderTemplate", typeof(DataTemplate), typeof(PivotHeaderView), new PropertyMetadata(null));



        public DataTemplate RightHeaderTemplate
        {
            get { return (DataTemplate)GetValue(RightHeaderTemplateProperty); }
            set { SetValue(RightHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightHeaderTemplateProperty =
            DependencyProperty.Register("RightHeaderTemplate", typeof(DataTemplate), typeof(PivotHeaderView), new PropertyMetadata(null));




        #endregion Dependency Properties

        #region Utilities

        private bool IsLoaded(FrameworkElement element)
        {
            return element == Window.Current.Content || VisualTreeHelper.GetParent(element) != null;
        }

        private void SetPivotHeaderVisibility(Pivot pivot, bool Visible)
        {
            var grid = pivot.VisualTreeFindName<Grid>("PivotLayoutElement");
            if (grid != null)
            {
                if (grid.RowDefinitions != null && grid.RowDefinitions.Count > 1)
                {
                    grid.RowDefinitions[0].Height = Visible ? new GridLength(0, GridUnitType.Auto) : new GridLength(0);
                }
            }
        }

        #endregion Utilities

    }
}
