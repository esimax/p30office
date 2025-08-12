using System;
using System.Windows;
using System.Windows.Data;

namespace POC.Module.SMS.Models
{
    public class ConvBoolToFlowDirection : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Boolean)) return value;

            if ((bool)value)
                return FlowDirection.RightToLeft;
            return FlowDirection.LeftToRight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
