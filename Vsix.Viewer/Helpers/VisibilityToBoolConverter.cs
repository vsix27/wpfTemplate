using System;
using System.Windows;
using System.Windows.Data;

namespace Vsix.Viewer.Helpers
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is Visibility ? (Visibility) value : Binding.DoNothing;
        }
    }

}
