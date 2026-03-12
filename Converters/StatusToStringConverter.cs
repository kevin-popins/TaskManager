using System;
using System.Globalization;
using System.Windows.Data;
using TaskManager.Models;

namespace TaskManager.Converters;

public class StatusToStringConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is TaskItemStatus s
            ? s == TaskItemStatus.Active ? "Активна" : "Завершена"
            : string.Empty;

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
