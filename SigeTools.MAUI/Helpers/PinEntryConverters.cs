using System.Globalization;

namespace SigeTools.MAUI.Helpers
{
    public class PinEntry1Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (String)value;
            if (!string.IsNullOrEmpty(s))
            {
                return s.Substring(0, 1);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PinEntry2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (String)value;
            if (!string.IsNullOrEmpty(s) && s.Length > 1)
            {
                return s.Substring(1, 1);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PinEntry3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (String)value;
            if (!string.IsNullOrEmpty(s) && s.Length > 2)
            {
                return s.Substring(2, 1);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PinEntry4Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (String)value;
            if (!string.IsNullOrEmpty(s) && s.Length > 3)
            {
                return s.Substring(3, 1);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
