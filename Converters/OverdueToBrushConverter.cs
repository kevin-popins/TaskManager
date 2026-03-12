using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TaskManager.Converters;

public class OverdueToBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is true
            ? new SolidColorBrush(Color.FromArgb(45, 220, 53, 69))
            : Brushes.Transparent;

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
