using System;
using System.Windows.Data;
using POL.Lib.Utils;
using POL.WPF.Controles.Conv;

namespace POC.Module.Calendar
{
    public class ConvCalendarMonthDate : IValueConverter
    {
        private ConvGlobalDate _Converter;
        private ConvGlobalDate Converter
        {
            get { return _Converter ?? (_Converter = new ConvGlobalDate {DateType = HelperLocalize.ApplicationCalendar}); }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return value != null ? Converter.Convert(value, null, "dd MMMM", null) : null;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
