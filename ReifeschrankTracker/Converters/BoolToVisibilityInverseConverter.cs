using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReifeschrankTracker.Converters;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility.Collapsed;
}
