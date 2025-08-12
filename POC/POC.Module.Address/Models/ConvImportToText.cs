using System;
using System.Windows.Data;

namespace POC.Module.Address.Models
{
    public class ConvImportToText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ImportAddressStructure)
            {
                var v = value as ImportAddressStructure;
                switch (v.ErrorType)
                {
                    case EnumImportErrorType.None:
                        return string.Empty;
                    case EnumImportErrorType.ErrorInContactTitle:
                        return "خطا در عنوان پرونده.";
                    case EnumImportErrorType.ErrorInvalidAddress:
                        return "آدرس معتبر نمی باشد.";
                    case EnumImportErrorType.ErrorInContactCode:
                        return "خطا در كد پرونده.";
                    default:
                        return "خطای نامشخص.";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
