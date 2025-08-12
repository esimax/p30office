using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Utils;
using POL.Lib.XOffice;
using POL.WPF.Controles.Conv;

namespace POL.Lib.Common
{
    public class ConvContactDate : IValueConverter
    {
        private ConvGlobalDate _Converter;

        private ConvGlobalDate Converter
        {
            get
            {
                if (_Converter == null)
                {
                    if (string.IsNullOrEmpty(HelperSettingsClient.ContactDateFormat))
                        HelperSettingsClient.ContactDateFormat = "yyyy/MM/dd";

                    _Converter = new ConvGlobalDate {DateType = HelperLocalize.ApplicationCalendar};
                }
                return _Converter;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value != null
                    ? Converter.Convert(value, null, HelperSettingsClient.ContactDateFormat, null)
                    : null;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
