using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Simple.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PerspectivePage : Page
    {
        public PerspectivePage()
        {
            this.InitializeComponent();
        }

        Visual Rect1Visual;
        Visual Rect2Visual;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Rect1Visual = ElementCompositionPreview.GetElementVisual(rect1);
            Rect2Visual = ElementCompositionPreview.GetElementVisual(rect2);
            Rect2Visual.Offset = new Vector3(200f, 0f, 0f);
        }

        private void rect1X_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.Offset = new Vector3((float)e.NewValue, Rect1Visual.Offset.Y, Rect1Visual.Offset.Z);
        }

        private void rect1Y_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.Offset = new Vector3(Rect1Visual.Offset.X, (float)e.NewValue, Rect1Visual.Offset.Z);
        }

        private void rect1Z_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.Offset = new Vector3(Rect1Visual.Offset.X, Rect1Visual.Offset.Y, (float)e.NewValue);
        }

        private void rect1CX_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.CenterPoint = new Vector3((float)e.NewValue, Rect1Visual.CenterPoint.Y, Rect1Visual.CenterPoint.Z);
        }

        private void rect1CY_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.CenterPoint = new Vector3(Rect1Visual.CenterPoint.X, (float)e.NewValue, Rect1Visual.CenterPoint.Z);
        }

        private void rect1CZ_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.CenterPoint = new Vector3(Rect1Visual.CenterPoint.X, Rect1Visual.CenterPoint.Y, (float)e.NewValue);
        }

        private void rect1R_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect1Visual == null) return;
            Rect1Visual.RotationAngleInDegrees = (float)e.NewValue;
        }

        private void rect2X_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.Offset = new Vector3((float)e.NewValue, Rect2Visual.Offset.Y, Rect2Visual.Offset.Z);
        }

        private void rect2Y_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.Offset = new Vector3(Rect2Visual.Offset.X, (float)e.NewValue, Rect2Visual.Offset.Z);
        }

        private void rect2Z_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.Offset = new Vector3(Rect2Visual.Offset.X, Rect2Visual.Offset.Y, (float)e.NewValue);
        }

        private void rect2CX_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.CenterPoint = new Vector3((float)e.NewValue, Rect2Visual.CenterPoint.Y, Rect2Visual.CenterPoint.Z);
        }

        private void rect2CY_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.CenterPoint = new Vector3(Rect2Visual.CenterPoint.X, (float)e.NewValue, Rect2Visual.CenterPoint.Z);
        }

        private void rect2CZ_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.CenterPoint = new Vector3(Rect2Visual.CenterPoint.X, Rect2Visual.CenterPoint.Y, (float)e.NewValue);
        }

        private void rect2R_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Rect2Visual == null) return;
            Rect2Visual.RotationAngleInDegrees = (float)e.NewValue;
        }

        private void rect1axisX_Checked(object sender, RoutedEventArgs e)
        {
            if (Rect1Visual == null) return;
            switch (((string)((RadioButton)sender).Content))
            {
                case "Axis X":
                    Rect1Visual.RotationAxis = Vector3.UnitX;
                    break;
                case "Axis Y":
                    Rect1Visual.RotationAxis = Vector3.UnitY;
                    break;
                case "Axis Z":
                    Rect1Visual.RotationAxis = Vector3.UnitZ;
                    break;
            }
        }

        private void rect2axisX_Checked(object sender, RoutedEventArgs e)
        {
            if (Rect2Visual == null) return;
            switch (((string)((RadioButton)sender).Content))
            {
                case "Axis X":
                    Rect2Visual.RotationAxis = Vector3.UnitX;
                    break;
                case "Axis Y":
                    Rect2Visual.RotationAxis = Vector3.UnitY;
                    break;
                case "Axis Z":
                    Rect2Visual.RotationAxis = Vector3.UnitZ;
                    break;
            }
        }

        private void rect1Reset_Click(object sender, RoutedEventArgs e)
        {
            rect1axisZ.IsChecked = true;
            rect1X.Value = 0;
            rect1Y.Value = 0;
            rect1Z.Value = 0;
            rect1CX.Value = 0;
            rect1CY.Value = 0;
            rect1CZ.Value = 0;
            rect1R.Value = 0;
        }

        private void rect2Reset_Click(object sender, RoutedEventArgs e)
        {
            rect2axisZ.IsChecked = true;
            rect2X.Value = 200;
            rect2Y.Value = 0;
            rect2Z.Value = 0;
            rect2CX.Value = 0;
            rect2CY.Value = 0;
            rect2CZ.Value = 0;
            rect2R.Value = 0;
        }

    }
}
