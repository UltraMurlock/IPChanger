using System.Globalization;
using System.Windows.Data;

namespace IPChanger.Converters
{
    public class IpConfigConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new IpConfig {
                DhcpEnabled = (bool)values[0],
                IpAddress = (string)values[1],
                SubnetMask = (string)values[2],
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            IpConfig ipConfig = (IpConfig)value;
            return new object[] {
                ipConfig.DhcpEnabled,
                ipConfig.IpAddress,
                ipConfig.SubnetMask
            };
        }
    }
}
