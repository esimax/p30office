using System;
using System.Globalization;
using System.Windows.Data;
using POL.DB.P30Office;
using POL.Lib.Utils;
using POL.WPF.Controles.Conv;

namespace POL.Lib.Common
{
    public class ConvCardTableEndingDate : IValueConverter
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
                var db = value as DBTMCardTable2;
                if (db.HasEndingDate)
                {
                    return Converter.Convert(db.EndingDate, null, "yyyy/MM/dd HH:mm", null);
                }
            }
            catch
            {
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
