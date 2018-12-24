﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
namespace MaterialLibs.Common
{
    public static class VisualTreeExtension
    {
        public static FrameworkElement VisualTreeFindName(this DependencyObject element, string name)
        {
            if (element == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            if (name.Equals((element as FrameworkElement)?.Name, StringComparison.OrdinalIgnoreCase))
            {
                return element as FrameworkElement;
            }
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var result = VisualTreeHelper.GetChild(element, i).VisualTreeFindName(name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public static T VisualTreeFindName<T>(this DependencyObject element, string name)
            where T : FrameworkElement
        {
            T retValue = null;
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var type = child as T;
                if (type != null && name.Equals(type.Name,StringComparison.OrdinalIgnoreCase))
                {
                    retValue = type;
                    break;
                }
                retValue = VisualTreeFindName<T>(child, name);
                if (retValue != null)
                {
                    break;
                }
            }
            return retValue;
        }

        public static T VisualTreeFind<T>(this DependencyObject element)
            where T : DependencyObject
        {
            T retValue = null;
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var type = child as T;
                if (type != null)
                {
                    retValue = type;
                    break;
                }
                retValue = VisualTreeFind<T>(child);
                if (retValue != null)
                {
                    break;
                }
            }
            return retValue;
        }

        public static IList<T> VisualTreeFindAll<T>(this DependencyObject element)
            where T : DependencyObject
        {
            List<T> retValues = new List<T>();
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var type = child as T;
                if (type != null)
                {
                    retValues.Add(type);
                }
                retValues.AddRange(VisualTreeFindAll<T>(child));
            }
            return retValues;
        }
    }
}
