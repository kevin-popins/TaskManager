using System;
using System.Globalization;
using System.Windows.Data;

namespace TaskManager.Converters;

public class DateToStringConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, CultureInfo c) =>
        value is DateTime dt ? dt.ToString("dd.MM.yyyy") : "—";

    public object ConvertBack(object v, Type t, object p, CultureInfo c) => throw new NotImplementedException();
}
