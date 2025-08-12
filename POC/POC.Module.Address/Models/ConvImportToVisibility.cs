using System;
using System.Windows;
using System.Windows.Data;

namespace POC.Module.Address.Models
{
    public class ConvImportToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImportAddressStructure)
            {
                var v = value as ImportAddressStructure;
                return v.ErrorType != EnumImportErrorType.None ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
