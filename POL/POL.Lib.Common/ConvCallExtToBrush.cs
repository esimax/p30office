using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace POL.Lib.Common
{
    public class ConvCallExtToBrush : IValueConverter 
    {
        private static List<int> CurrentUserCode { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
