using System;
using System.Globalization;
using System.Windows.Data;
using POL.Lib.Utils;
using POL.WPF.Controles.Conv;

namespace POL.Lib.Common
{
    public class ConvTransDate : IValueConverter
    {
        private ConvGlobalDate _Converter;

        private ConvGlobalDate Converter
        {
            get
            {
                if (_Converter == null)
                {

                    _Converter = new ConvGlobalDate {DateType = HelperLocalize.ApplicationCalendar};
                }
                return _Converter;
            }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value == null
                    ? null
                    : Converter.Convert(value, null, "yyyy/MM/dd HH:mm:ss" , null);
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
