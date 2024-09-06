using System.Globalization;
using System.Windows.Data;

namespace IPChanger.Converters
{
    public class DhpcFlagToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Динамический IP" : "Статический IP";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
