using System;
using System.Windows.Data;

namespace POC.Module.Profile.Models
{
    public class ConvTemplateTime : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is int)
                {
                    var i = System.Convert.ToInt32(value);
                    var h = (int)(i / 100);
                    var m = i % 100;

                    return string.Format("{0}:{1}",
                                         h.ToString().PadLeft(2, '0'),
                                         m.ToString().PadLeft(2, '0'));
                }
            }
            catch
            {
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
