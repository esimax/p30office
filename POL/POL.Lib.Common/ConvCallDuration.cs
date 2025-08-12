using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.XOffice;

namespace POL.Lib.Common
{
    public class ConvCallDuration : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var i = System.Convert.ToInt32(value);
                if (i < 0)
                    return ConstantGeneral.MissCallTitle;
                var ts = TimeSpan.FromSeconds(System.Convert.ToInt32(value));
                return ts.ToString();
            }
            catch
            {
                return ConstantGeneral.MissCallTitle;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
