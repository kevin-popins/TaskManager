using System;
using System.Globalization;
using System.Windows.Data;
using TaskManager.Models;

namespace TaskManager.Converters;

public class PriorityToDisplayConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is TaskPriority pr ? pr switch
        {
            TaskPriority.High => "Высокий",
            TaskPriority.Medium => "Средний",
            TaskPriority.Low => "Низкий",
            _ => value.ToString()!
        } : string.Empty;

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
