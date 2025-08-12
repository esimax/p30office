using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace POL.Lib.Common
{
    public class ConvBoolToFontWeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToBoolean(value) ? FontWeights.Bold : FontWeights.Normal;
            }
            catch
            {
                return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
