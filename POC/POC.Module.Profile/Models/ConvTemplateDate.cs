using System;
using System.Windows.Data;
using POL.Lib.Interfaces;
using POL.Lib.Utils;

namespace POC.Module.Profile.Models
{
    public class ConvTemplateDate : IValueConverter
    {
        public ConvTemplateDate()
        {
            CalType = EnumCalendarType.ApplicationSettings;
        }

        public EnumCalendarType CalType { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is DateTime)
                {
                    var i = System.Convert.ToDateTime(value);
                    return HelperLocalize.DateTimeToString(i, CalType, HelperLocalize.ApplicationDateTimeFormat);
                }
            }
            catch
            {
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


