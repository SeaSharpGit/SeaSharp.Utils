using System;
using System.Globalization;
using System.Windows.Data;

namespace SeaSharp.Utils.Wpf.Converters
{
    public class ObjectToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? typeof(object) : value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("ObjectToTypeConverter不支持ConvertBack");
        }
    }
}