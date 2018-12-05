using System;
using System.Windows.Data;

namespace SeaSharp.Utils.Wpf.Converters
{
    public class FileSizeToStringConverter : IValueConverter
    {
        private const int _X = 1024;
        private const int _2X = 1024 * 1024;
        private const int _3X = 1024 * 1024 * 1024;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var size = System.Convert.ToInt64(value);
            if (size <= _X)
            {
                return size.ToString() + "B";
            }
            else if (size <= _2X)
            {
                return (size / _X).ToString() + "KB";
            }
            else if (size < _3X)
            {
                return (size / _2X).ToString() + "MB";
            }
            else
            {
                return (size / _3X).ToString() + "GB";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("FileSizeToStringConverter不支持ConvertBack");
        }
    }
}