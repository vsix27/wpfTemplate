using System;
using System.Windows;
using System.Windows.Data;

namespace WpfDemoWithMvvmp.Helpers
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                bool visible = (bool)value;
                return (visible) ? Visibility.Visible : Visibility.Hidden;
            }
            else
                return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                var visibility = (Visibility)value;
                return visibility;
            }
            return Binding.DoNothing;
        }
    }

}
