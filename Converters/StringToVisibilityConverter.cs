using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TaskManager.Converters;

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
