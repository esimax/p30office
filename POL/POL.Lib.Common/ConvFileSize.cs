using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Utils;

namespace POL.Lib.Common
{
    public class ConvFileSize : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var i = System.Convert.ToDecimal(value);
                return i < 0 ? "0" : HelperConvert.ConvertToFileSizeFormat(i);
            }
            catch
            {
                return "?? MB";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
