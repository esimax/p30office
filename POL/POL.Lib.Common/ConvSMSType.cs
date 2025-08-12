using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using POL.Lib.Interfaces;

namespace POL.Lib.Common
{
    public class ConvSMSType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumSmsType)
            {
                var ct = (EnumSmsType) value;
                switch (ct)
                {
                    case EnumSmsType.Receive:
                        return Brushes.LightGreen;
                    case EnumSmsType.RequestToSend:
                        return Brushes.LightGoldenrodYellow;
                    case EnumSmsType.SendDone:
                        return Brushes.LightPink;
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
