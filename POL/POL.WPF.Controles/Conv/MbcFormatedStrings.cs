using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace POL.WPF.Controles.Conv
{
    public class MbcFormatedStrings : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var rv = string.Empty;
            if (parameter == null) return rv;

            var l = (from n in values select n is string ? n.ToString() : string.Empty).ToArray();

            var format = parameter.ToString();
            format = format.Replace("[[", "{").Replace("]]", "}");
            return string.Format(format, l);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
