using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using POL.Lib.Interfaces.SmsSettings;

namespace POL.Lib.Common
{
    public class ConvSmsStatusBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumSmsStatus)
            {
                var ct = (EnumSmsStatus) value;
                switch (ct)
                {
                    case EnumSmsStatus.Disabled:
                        return Brushes.LightGray;
                    case EnumSmsStatus.Success:
                        return Brushes.LightGreen;
                    case EnumSmsStatus.Faile:
                        return Brushes.Tomato;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
