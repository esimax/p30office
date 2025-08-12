using System;
using System.Globalization;
using System.Windows.Data;

namespace POC.Module.Calendar
{
    public class TimeSpanToDateTimeConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is TimeSpan))
                {
                    return null;
                }
                var span = (TimeSpan)value;
                return new DateTime(span.Ticks + DateTime.Now.Date.Ticks);
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!(value is DateTime))
                {
                    return null;
                }
                var time = (DateTime)value;
                var d1 = time.Date;

                return TimeSpan.FromTicks(time.Ticks - d1.Ticks);
            }
            catch
            {
                return value;
            }
        }
    }
}
