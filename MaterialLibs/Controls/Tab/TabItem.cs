using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace MaterialLibs.Controls.Tab
{
    [ContentProperty(Name = "Content")]
    public sealed class TabItem : ContentControl, ITabItem
    {
        public TabItem()
        {
            this.DefaultStyleKey = typeof(TabItem);
        }

        ContentPresenter ContentPresenter;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ((ITabItem)(this)).UpdateLoadState(false);
        }

        private ContentPresenter GetContentPresenter()
        {
            return (ContentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter);
        }

        void ITabItem.UpdateLoadState(bool Load)
        {
            if (Load)
            {
                VisualStateManager.GoToState(this, "Load", true);
                GetContentPresenter();
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
                if (ContentPresenter != null)
                {
                    XamlMarkupHelper.UnloadObject(ContentPresenter);
                    ContentPresenter = null;
                }
            }
        }

        public bool UnloadItemOutsideViewport
        {
            get { return (bool)GetValue(UnloadItemOutsideViewportProperty); }
            set { SetValue(UnloadItemOutsideViewportProperty, value); }
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty UnloadItemOutsideViewportProperty =
            DependencyProperty.Register("UnloadItemOutsideViewport", typeof(bool), typeof(TabItem), new PropertyMetadata(false));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TabItem), new PropertyMetadata(null));


    }
}
