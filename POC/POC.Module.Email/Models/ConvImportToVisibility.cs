using System;
using System.Windows;
using System.Windows.Data;


namespace POC.Module.Email.Models 
{
    public class ConvImportToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImportEmailStructure)
            {
                var v = value as ImportEmailStructure;
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
