using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskManager.Models;

namespace TaskManager.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is TaskPriority pr ? pr switch
        {
            TaskPriority.High => new SolidColorBrush(Color.FromRgb(220, 53, 69)),
            TaskPriority.Medium => new SolidColorBrush(Color.FromRgb(255, 193, 7)),
            TaskPriority.Low => new SolidColorBrush(Color.FromRgb(40, 167, 69)),
            _ => Brushes.Gray
        } : Brushes.Gray;

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
