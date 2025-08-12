using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using POL.Lib.Interfaces;

namespace POL.Lib.Common
{
    public class ConvCallType : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumCallType)
            {
                var ct = (EnumCallType) value;
                return ct == EnumCallType.CallIn ? Brushes.LightGreen : Brushes.LightPink;
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
